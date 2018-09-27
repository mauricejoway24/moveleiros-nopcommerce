(function (window) {

    'use strict';

    window.getProducts = getProducts;

    // Gets and returns a list of stores
    function getProducts(curl, successCallback, failCallback, doneCallback) {

        var queryString = curl.length > 1 ? curl + '&ajax=true' : curl;

        console.log('test')

        $.ajax({
            url: '/catalog/search' + queryString,
            type: 'GET',
            beforeSend: function (xhr) { xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest'); },
            success: function (data, status) {

                if (status !== "success") {
                    if (failCallback)
                        failCallback();

                    return;
                }

                if (successCallback)
                    successCallback(data);
            }
        })
        .fail(function () {
            if (failCallback)
                failCallback();
        })
        .done(function () {
            if (doneCallback)
                doneCallback();
        });
    }

})(window);