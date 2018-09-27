'use strict';

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ('value' in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError('Cannot call a class as a function'); } }

var Soixa = (function () {
    function Soixa() {
        _classCallCheck(this, Soixa);
    }

    _createClass(Soixa, [{
        key: 'get',
        value: function get(url, callback, headers) {
            var req = new XMLHttpRequest();

            req.onreadystatechange = function () {
                if (req.readyState == XMLHttpRequest.DONE) {
                    if (callback) callback(null, req.response);
                }
            };

            req.onerror = function (err) {
                if (callback) callback(err, null);
            };

            req.open('GET', url, true);

            for (key in headers) {
                req.setRequestHeader(key, headers[key]);
            }

            req.setRequestHeader('X-Requested-With', 'XMLHttpRequest');

            req.send(null);
        }
    }, {
        key: 'post',
        value: function post(url, data, callback, headers) {
            var req = new XMLHttpRequest();

            req.onreadystatechange = function () {
                if (req.readyState == XMLHttpRequest.DONE) {
                    if (callback) callback(null, req.response);
                }
            };

            req.onerror = function (err) {
                if (callback) callback(err, null);
            };

            for (key in headers) {
                req.setRequestHeader(key, headers[key]);
            }

            req.open('POST', url, true);
            req.send(data);
        }
    }]);

    return Soixa;
})();

var FilterRequest = (function () {
    function FilterRequest() {
        _classCallCheck(this, FilterRequest);

        this._axios = new Soixa();

        this.bindEvent();
    }

    _createClass(FilterRequest, [{
        key: 'bindEvent',
        value: function bindEvent() {
            var _this = this;

            window.getProducts = function (c, s, f, d) {
                return _this.getProducts(c, s, f, d);
            };
        }
    }, {
        key: 'getProducts',
        value: function getProducts(curl, successCallback, failCallback, doneCallback) {
            var queryString = curl.length > 1 ? curl + '&ajax=true' : curl;

            this._axios.get('/catalog/search' + queryString, function (err, data) {
                if (err) {
                    if (failCallback) failCallback(err);
                    return;
                }

                if (successCallback) successCallback(data);
                if (doneCallback) doneCallback();
            });
        }
    }]);

    return FilterRequest;
})();

new FilterRequest();

