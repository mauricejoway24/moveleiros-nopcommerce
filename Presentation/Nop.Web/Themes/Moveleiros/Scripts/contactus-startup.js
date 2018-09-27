(function (w, d, s, u) {
    w.MoveleirosChat = {}; w.MoveleirosChat.iframeUrl = u;
    w.MoveleirosChat.h = '500px';
    w.MoveleirosChat.ci = true; w.MoveleirosChat.dt = '#contact-us-chat';
    var h = d.getElementsByTagName(s)[0], j = d.createElement(s);
    j.async = true; //j.src = baseChatUrl + '/external-plugin.js';
    j.src = 'https://chat.moveleiros.com.br/external-plugin.js';
    h.parentNode.insertBefore(j, h);
})(window, document, 'script', baseChatUrl + '/#/livechat?relogin=true&storeId=' + defaultStoreId);
