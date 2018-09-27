'use strict';

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ('value' in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError('Cannot call a class as a function'); } }

var Sidebar = (function () {
    function Sidebar(trigger, popupElement, direction) {
        _classCallCheck(this, Sidebar);

        this.trigger = trigger;
        this.popupElement = popupElement;
        this.direction = direction;

        this._init();
    }

    _createClass(Sidebar, [{
        key: '_init',
        value: function _init() {
            var _this = this;

            var btnElement = document.querySelector(this.trigger);
            var pop = document.querySelector(this.popupElement);

            if (!btnElement || !pop) return;

            pop.addEventListener('click', function () {
                _this.close();
            });
            btnElement.addEventListener('click', function () {
                _this.show(_this);
            });
        }
    }, {
        key: 'show',
        value: function show(ctx) {
            var c = ctx || this;
            var popupElement = document.querySelector(c.popupElement);

            document.body.classList.add('open');

            if (this.direction === 'right') document.body.classList.add('open-right');else document.body.classList.add('open-left');
        }
    }, {
        key: 'close',
        value: function close() {
            document.body.classList.remove('open');
            document.body.classList.remove('open-right');
            document.body.classList.remove('open-left');
        }
    }]);

    return Sidebar;
})();

