// Filter behaviour
(function (window) {

    'use strict';

    var filterItems;
    var currentSearchUrl;
    var lastViewModel = 'list';
    var lastQuery = '';
    var orderBy = '0';
    var lastCity = '';

    // Assets
    var timeoutHandler = null;
    var dataFilterAttr = 'data-filter-type';
    var loadingCover;

    $(init);
    
    function init() {

        loadVars();
        bindEvents();

        window.setFilterTimer = setFilterTimer;
        window.setLastCity = setLastCity;
    }

    function setLastCity(value) {
        lastCity = value;
    }

    function reload() {

        init();
        window.AccordionControl.reload();
    }

    function loadVars() {

        filterItems = $('[' + dataFilterAttr + ']');
        loadingCover = $('div.banner-cover');

        currentSearchUrl = window.location.search;
    }

    function setUrl(currentParams) {

        window.history.replaceState('', document.title, currentParams);
    }

    function handleUrlSearch(paramsObj) {

        var equalizing = [];

        for (var ix in paramsObj) {

            for (var key in paramsObj[ix]) {

                equalizing.push(key + '=' + paramsObj[ix][key]);
            }
        }

        currentSearchUrl = '?' + equalizing.join('&');

        setUrl(currentSearchUrl);

        return currentSearchUrl;
    }
    
    function bindEvents() {

        filterItems.each(function (ix, elem) {

            var filterType = $(elem).attr(dataFilterAttr);

            switch (filterType) {

                case 'checkbox': 
                    bindCheckBoxEvent(elem);
                    break;

                case 'viewmode':
                    bindViewModelEvent(elem);
                    break;

                case 'keyword':
                    bindKeywordEvent(elem);
                    break;

                case 'city':
                    bindCityEvent(elem);
                    break;

                case 'orderby':
                    bindOrderByEvent(elem);
                    break;

                case 'number':
                    bindNumberEvent(elem);
                    break;

                default:
            }
        });

        // See all filters
        $('[data-see-all]').off('click');
        $('[data-see-all]').on('click', seeAllHandler);

        $('.sidebar #sidebar-wrapper').off('click');
        $('.sidebar #sidebar-wrapper').on('click', function (e) { e.stopPropagation(); });

        window.afterSelectCategories = afterSelectCategories;
        window.afterSelectBrands = afterSelectBrands;
    }

    function afterSelectCategories(selectedCats) {

        selectedCats.each(function (id, elem) {

            var $this = $(elem);
            var name = $this.attr('name');
            var value = $this.attr('value');

            $('.filter, .sidebar')
                .find('input[type=checkbox][name=' + name + '][value=' + value + ']')
                .iCheck(elem.checked ? 'check' : 'uncheck')
                .iCheck('update');
        });

        setFilterTimer(true);
    }

    function afterSelectBrands(selectedBrands) {

        selectedBrands.each(function (id, elem) {

            var $this = $(elem);
            var name = $this.attr('name');
            var value = $this.attr('value');

            $('.filter, .sidebar')
                .find('input[type=checkbox][name=' + name + '][value=' + value + ']')
                .iCheck(elem.checked ? 'check' : 'uncheck')
                .iCheck('update');
        });

        setFilterTimer(true);
    }

    function seeAllHandler(e) {

        e.preventDefault();

        var link = $(this).attr('data-see-all');
        var title = $(this).attr('data-see-all-title');
        var err = alert;
        var m = 'Erro ao tentar carregar informações. Recarregue a página e tente novemente. Caso não consiga, entre em contato conosco.';

        if (link === '') {

            err(m);
            return;
        }

        window.MovScripts.openPopup(title, null, function () {

            window.MovScripts.setPopupContent(document.createElement('div'));
        });

        window.MovScripts.getAsync(link + currentSearchUrl, function (data) {

            window.MovScripts.setPopupContent(data);

        }, function () {

            err(m);
        });
    }

    function bindNumberEvent(elem) {

        $(elem)
            .off('keyup')
            .on('keyup', function (event) {

                var $this = $(this);
                var name = $this.attr('data-filter');
                var value = $this.attr('value');

                $('[type=number][data-filter=' + name + ']').val(value);

                setFilterTimer(null, 1000);
            });
    }

    function bindCheckBoxEvent(elem) {

        var $elem = $(elem);

        $elem
            .find('input[type=checkbox]')
            .off('ifChanged')
            .on('ifChanged', function (event) {

                var $this = $(this);
                var name = $this.attr('name');
                var value = $this.attr('value');

                $('.filter, .sidebar')
                    .find('input[type=checkbox][name=' + name + '][value=' + value + ']')
                    .prop('checked', this.checked ? 'checked' : '');

                setFilterTimer(true);
            });
    }

    function bindKeywordEvent(elem) {

        var $elem = $(elem);

        $elem
            .find('input[type=text]')
            .off('keyup')
            .on('keyup', function (event) {

                lastQuery = $(this).val();

                setFilterTimer(null, 1000);
            });
    }

    function bindCityEvent(elem) {
        var $elem = $(elem);

        $elem
            .find('input[type=text]')
            .off('keyup')
            .on('keyup', function (event) {

                lastCity = $(this).val();

                setFilterTimer(null, 1000);
            });
    }

    function bindViewModelEvent(elem) {

        var $elem = $(elem);

        lastViewModel = $elem
            .find('a[data-filter-value].selected')
            .attr('data-filter-value');

        $elem
            .find('a[data-filter-value]')
            .off('click')
            .on('click', function (event) {

                event.preventDefault();

                var $a = $(this);
                var typeOfView = $a.attr('data-filter-value');

                if (typeOfView === lastViewModel)
                    return;

                // Remove selected from all anchors
                $a
                    .parent()
                    .find('a[data-filter-value]')
                    .removeClass('selected');

                // Add selected to this anchor
                $a.addClass('selected');

                lastViewModel = typeOfView;

                setFilterTimer(null);
            });
    }

    function bindOrderByEvent(elem) {

        var $elem = $(elem);

        // .off = turn old events off
        $elem
            .find('select')
            .off('change')
            .on('change', function (event) {

                event.preventDefault();

                var val = $(this).val();
                var startAt = val.indexOf('orderby');

                orderBy = val
                    .substr(startAt)
                    .split('=')[1];

                setFilterTimer(null);
            });
    }

    function prepareParams() {

        var formParams = [];

        // Check if no category is selected
        if ($('div[name=category] :input:checkbox:checked').length == 0) {
            $('div[name=subcategory] :input:checkbox:checked').remove();
        }

        // Checkboxes
        var checks = $(':input:checkbox:checked');

        var checkIfExists = function (name, value) {

            return formParams.some(function (item, ix) {
                for (var prop in item) {
                    if (prop === name) {

                        if (item[prop] === value)
                            return true;
                    }
                }

                return false;
            });
        };

        var addKeyValue = function (elem, nameAttr, valAttr) {

            var $el = $(elem);
            var p = {};
            var name = $el.attr(nameAttr || 'name');
            var value = $el.attr(valAttr || 'value');
            var exists = checkIfExists(name, value);

            if (!exists) {
                p[name] = value;
                formParams.push(p);
            }
        };

        checks.each(function (ix, elem) {
            addKeyValue(elem);
        });

        // Viewlist
        formParams.push({ 'viewmode': lastViewModel });

        // q
        if (lastQuery !== '')
            formParams.push({ 'q': lastQuery });

        // orderby
        if (orderBy !== '0')
            formParams.push({ 'orderby': orderBy });

        // money
        $('[data-filter-type=number]').each(function (ix, elem) {
            addKeyValue(elem, 'data-filter');
        });

        // city
        if (lastCity !== '')
            formParams.push({ 'city': lastCity });

        return formParams;
    }

    function setFilterTimer(noScroll, time) {

        if (timeoutHandler)
            clearTimeout(timeoutHandler);

        if (noScroll)
            $("html, body").animate({ scrollTop: 0 }, "slow");

        loadingCover.fadeIn(500);

        var curl = handleUrlSearch(prepareParams());

        timeoutHandler = setTimeout(function () {

            window.getProducts(curl, fillProducts, failGettingProducts, doneProducts);
        }, time || 500);
    }

    function fillProducts(data) {

        var $data = $(data);

        $('div.page.search-page').html('');

        var sub = $data.find('[name="subcategory"]');

        $('[data-accordion-group] [name="subcategory"]').remove();

        sub.show();
        sub.insertAfter('[data-accordion-group] [name="category"]');

        $('[data-accordion-group] [name="subcategory"]').find('input[type=checkbox], input[type=radio]').iCheck({
            checkboxClass: 'icheckbox_square-blue',
            radioClass: 'iradio_square-blue',
            increaseArea: '30%'
        });

        reload();

        // Check if remove categories is needed
        var checks = $('[data-accordion-group] [name="category"] :input:checkbox:checked');

        if (checks.length === 0) {
            $('[data-accordion-group] [name="subcategory"]').remove();
        }

        $data.find(sub).remove();

        $('div.page.search-page').html($data);
    }

    function failGettingProducts() {

        var errorMessage = typeof MOV_LOAD_ERROR !== "undefined" ? MOV_LOAD_ERROR :
            'Erro ao tentar filtrar informações. Recarregue a página e tente novamente. Caso não consiga, entre em contato conosco.';

        alert(errorMessage);
    }

    function doneProducts() {
        loadingCover.fadeOut(500);
    }

    function filterAsync() {
        setFilterTimer();
    }

    function generateJsonParam() {
        var resultData = {};

        setFilterFromCheckboxes(resultData);

        return resultData;
    }

    function setFilterFromCheckboxes(resultData) {

        checkboxItems.each(function (ix, elem) {
            var $elem = $(elem);
            var prop = $elem.attr('data-filter');

            var propsArr = [];

            $elem
                .find('input[type = checkbox]')
                .each(function () {

                    if (this.checked)
                        propsArr.push(this.getAttribute('value'));
                });

            if (propsArr.length === 0) {

                resultData[prop] = '';
                return;
            }

            resultData[prop] = propsArr.reduce(function (previous, current) {
                return previous + ', ' + current;
            });
        });
    }

    function showFilterPanel() {

        if (hideMobileFilterPanel())
            return;

        filterPanelEl
            .removeClass('hidden-xs')
            .addClass('hideUp')
            .attr('style', 'display: none;')
            .slideDown(500);
    }

})(window);

// City autocomplete
//(function (window) {

//    'use strict';

//    var el = $('#CidadeDescricaoFilter');
//    var normalizado = $('#CidadeFilter');

//    var options = {

//        url: function (descricao) {
//            return "/cidade/listarpordescricao";
//        },

//        getValue: function (element) {
//            return element.Descricao + ', ' + element.UFCodigo;
//        },

//        ajaxSettings: {
//            dataType: "json",
//            method: "GET",
//            data: {
//                dataType: "json"
//            }
//        },

//        preparePostData: function (data) {
//            data.descricao = el.val();
//            return data;
//        },

//        list: {
//            onChooseEvent: function () {

//                var selectedEl = el.getSelectedItemData();

//                normalizado.val(selectedEl.CidadeNormalizada);

//                if (window.filterAsync)
//                    window.filterAsync();
//            }
//        },

//        requestDelay: 500,
//        adjustWidth: false
//    };

//    el.easyAutocomplete(options);
//})(window);