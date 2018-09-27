'use strict';

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ('value' in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError('Cannot call a class as a function'); } }

var AccordionItem = (function () {
    function AccordionItem() {
        _classCallCheck(this, AccordionItem);

        this._el = null;
        this._disable = false;
    }

    _createClass(AccordionItem, [{
        key: 'disableAccordion',
        value: function disableAccordion() {
            this._disable = true;
        }
    }, {
        key: 'setElement',
        value: function setElement(element) {
            var _this = this;

            this._el = element;
            this._el.addEventListener('click', function () {
                return _this.onClick();
            });
        }
    }, {
        key: 'onClick',
        value: function onClick() {
            if (this._disable) return;

            var parent = this._el.parentElement;
            var elClass = parent.classList;

            if (elClass.contains('open')) elClass.remove('open');else elClass.add('open');
        }
    }]);

    return AccordionItem;
})();

var Accordion = (function () {
    function Accordion() {
        _classCallCheck(this, Accordion);

        this._elements = [];

        this.bindEvents();
    }

    _createClass(Accordion, [{
        key: 'bindEvents',
        value: function bindEvents() {
            var _this2 = this;

            document.querySelectorAll('[data-accordion] [data-control]').forEach(function (accordion) {
                var el = new AccordionItem();
                el.setElement(accordion);
                _this2._elements.push(el);
            });
        }
    }, {
        key: 'reload',
        value: function reload() {
            this._elements.forEach(function (accordion) {
                accordion.disableAccordion();
            });
            this._elements = [];
            this.bindEvents();
        }
    }]);

    return Accordion;
})();

window.addEventListener('load', function (_) {
    window.AccordionControl = new Accordion();
});

