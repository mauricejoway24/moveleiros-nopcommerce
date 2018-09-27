(function (w) {

    'use strict'

    var canUseChat = false
    var chatOpen = false
    var modalId = '#modalCadastro'
    var modalHideEvent = 'hide.bs.modal'
    var modalShownEvent = 'shown.bs.modal'
    var alreadyConverted = false
    var conversionEvent = null

    w.MovScripts = w.MovScripts || {}

    window.addEventListener('load', init)
    bindEvent(window, 'message', function (e) {
        if (e.origin === baseChatUrl && e.data === 'EndChatEvent') {
            var popup = $(modalId);
            popup.find('iframe').remove()
            if (!chatOpen) return
            popup.modal('hide')
            if (conversionEvent) {
                clearTimeout(conversionEvent)
                conversionEvent = null
            }
            if (window.location.pathname !== '/') {
                window.location.href = window.location.origin
            }
            console.log(window.location.href);
        }        
    });

    function bindEvent(element, eventName, eventHandler) {
        if (element.addEventListener) {
            element.addEventListener(eventName, eventHandler, false);
        } else if (element.attachEvent) {
            element.attachEvent('on' + eventName, eventHandler);
        }
    }

    function init() {
        w.MovScripts.openChat = openChat
        w.MovScripts.openProfileChat = openProfileChat
    }

    function openProfileChat(storeId, lojistaName) {
        openChat(storeId, null, null, null, null, {
            email: '',
            origem: 'Perfil',
            nomeLoja: lojistaName,
            storeId: storeId
        });
    }

    function openChat(storeId, codProd, descProd, codCity, lojistaId, extraOpts, productInfo = null, seName = "") {
        var popup = $(modalId);

        popup.find('iframe').remove()

        popup.on(modalHideEvent, function (e) {
            popup.off(modalHideEvent)

            chatOpen = false

            MoveleirosChat.closeChat()
        });

        popup.on(modalShownEvent, function (e) {
            console.log('calling chat', codCity, lojistaId)

            popup.off(modalShownEvent)

            popup.find('.loading').addClass('show')
            if (productInfo) {
                MoveleirosChat.setDefaultValues({
                    useHeaderToolbar: true,
                    extra: {
                        lojistaId: lojistaId,
                        codCity: codCity,
                        email: '',
                        origem: 'Mktplace',
                        codProd: productInfo.Id ? productInfo.Id : productInfo.ProductId,
                        product: productInfo.Name ? productInfo.Name : productInfo.ProductName,
                        productSeName: window.location.origin + '/' + (seName ? seName : productInfo.SeName)
                    }
                })
            } else {
                if (extraOpts) {
                    MoveleirosChat.setDefaultValues({
                        useHeaderToolbar: true,
                        extra: extraOpts
                    })
                } else {

                    MoveleirosChat.setDefaultValues({
                        useHeaderToolbar: true,
                        extra: {
                            lojistaId: lojistaId,
                            codCity: codCity,
                            email: '',
                            origem: 'Mktplace',
                            codProd: codProd,
                            product: descProd
                        }
                    })
                }
            }            

            MoveleirosChat.createIframe({
                width: '100%',
                height: '100%',
                borderRadius: '10px',
                url: baseChatUrl + '/#/livechat?relogin=true&storeId=' + storeId,
                target: '#modalBodyContainer',
                destroyAfterClose: true
            })

            chatOpen = true
        });

        popup.modal()
    }

    MoveleirosChat.onChatLoad = function () {
        var popup = $(modalId)
        popup.find('.loading').removeClass('show')

        if (conversionEvent) {
            clearTimeout(conversionEvent)
            conversionEvent = null
        }

        conversionEvent = setTimeout(function () {
            if (!window.dataLayer)
                window.dataLayer = [];

            window.dataLayer.push({ 'event': 'conversaoOrcamentoOk' });

            conversionEvent = null
        }, 20000)
    }

    MoveleirosChat.onScriptLoad = function () {
        console.log('script loaded')
    }

    MoveleirosChat.onChatClose = function () {
        var popup = $(modalId)

        if (!chatOpen) return

        popup.modal('hide')

        if (conversionEvent) {
            clearTimeout(conversionEvent)
            conversionEvent = null
        }
    }

})(window);