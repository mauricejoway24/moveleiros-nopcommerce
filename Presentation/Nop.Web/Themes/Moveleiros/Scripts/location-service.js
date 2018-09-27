(function (w) {

    'use strict'

    w.MovScripts = w.MovScripts || {}

    w.initReferenceMap = init

    var loadingElement

    function init() {
        w.MovScripts.getPosition = getPosition

        if (!navigator.geolocation) {
            $('[data-geonotification]').remove()
        }
    }

    function getPosition(locationEl) {
        var cityEl = locationEl.closest('#City')
        loadingElement = locationEl.siblings('i')
        loadingElement.show()

        navigator
            .geolocation
            .getCurrentPosition(getSuccessFunction, function (error) {
                console.log(error)
                if (error.code === 1) {
                    alert('O Marketplace Moveleiros necessita permissão para utilizar sua localização antes de realizar essa ação.')
                    loadingElement.hide()
                } else {
                    getErrorFunction()
                }
            })
    }

    function getSuccessFunction(position) {
        var lat = position.coords.latitude
        var lng = position.coords.longitude
        var city
        var geocoder = new google.maps.Geocoder()
        var latlng = new google.maps.LatLng(lat, lng)

        geocoder.geocode({ 'latLng': latlng }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {

                if (results[1]) {

                    //find country name
                    for (var i = 0; i < results[0].address_components.length; i++) {
                        for (var b = 0; b < results[0].address_components[i].types.length; b++) {

                            //there are different types that might hold a city admin_area_lvl_1 usually does in come cases looking for sublocality type will be more appropriate
                            if (results[0].address_components[i].types[b] == "locality") {
                                //this is the object you are looking for
                                var city = results[0].address_components[i];
                                break;
                            }
                        }
                    }

                    //city data
                    $('[data-city]').val(city.long_name)
                    setLastCity(city.long_name)
                    loadingElement.hide()
                    w.setFilterTimer()
                } else {
                    // No results
                }
            } else {
                getErrorFunction()
            }
        })
    }

    function getErrorFunction() {
        loadingElement.hide()
        alert("Erro ao tentar carregar informações de localização. Tente digitar sua cidade.")
    }

})(window)