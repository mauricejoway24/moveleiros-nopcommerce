(function (w) {

    w.MovScripts = w.MovScripts || {};

    w.MovScripts.getAsync = getAsync;
    w.MovScripts.getScript = getScript;

    function getAsync(url, successCallback, failCallback, doneCallback) {

        $.get(url,
            function (data, status) {

                if (status !== "success") {
                    if (failCallback)
                        failCallback();

                    return;
                }

                if (successCallback)
                    successCallback(data);
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

    function getScript(url, successCallback, failCallback, doneCallback) {

        $.ajax({
            url: url,
            dataType: 'script',
            success: successCallback,
            error: function (err) {

                if (err && err.status && err.status == 200) {

                    successCallback(err.responseText);
                    return;
                }

                if (failCallback)
                    failCallback(err);
            },
            async: true
        })
        .done(function () {
            if (doneCallback)
                doneCallback();
        });
    }

})(window);