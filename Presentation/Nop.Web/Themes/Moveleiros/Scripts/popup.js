(function (w) {

    'use strict';

    w.MovScripts = w.MovScripts || {};

    w.MovScripts.openPopup = openPopup;
    w.MovScripts.closePopup = closePopup;
    w.MovScripts.setPopupContent = setPopupContent;

    var popupWrapper = $();
    var _currentCloseCallback;

    $(init);

    function init() {

        popupWrapper = $('div.wrapper-popup');

        popupWrapper
            .find('a[data-close]')
            .on('click', closePopup);
    }

    function openPopup(title, content, closeCallback) {

        _currentCloseCallback = closeCallback;
        
        popupWrapper
            .find('.title h2')
            .html(title);

        popupWrapper
            .find('.content')
            .hide();

        popupWrapper
            .find('.loading')
            .show();

        if (content) {
            setPopupContent(content);
        }

        popupWrapper.show(100);

        $(window).scrollTop(0);
        $(document.body).addClass('open');
    }

    function setPopupContent(content) {

        popupWrapper
            .find('.loading')
            .hide();

        popupWrapper
            .find('.content')
            .html(content)
            .show();
    }

    function closePopup() {

        popupWrapper.hide(100);

        $('body').removeClass('open');

        if (_currentCloseCallback)
            _currentCloseCallback();
    }

})(window);