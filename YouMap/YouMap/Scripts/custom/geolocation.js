YouMap.Geolocation = function ($) {

    var initialLocation;
    var browserSupportFlag = new Boolean();
    var zoomOnUser = 17;

    var initialize = function() {

    };

    var locate = function(callback, errorCallback) {
        var map = getMap();
        if (navigator.geolocation) {
            browserSupportFlag = true;
            return locateW3C(callback, errorCallback);
        } else if (google.gears) {
            browserSupportFlag = true;
            return locateGoogleGears(callback);
        } else {
            browserSupportFlag = false;
            handleNoGeolocation(browserSupportFlag);
        }
        ;
    };

    var getLastLocation = function() {
        return initialLocation;
    };

    var locateW3C = function(callback, errorCallback) {
        navigator.geolocation.getCurrentPosition(function(position) {
            initialLocation = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
            callback(initialLocation);
        }, function() {
            handleNoGeolocation(browserSupportFlag);
            errorCallback(error);
        });
    };


    var locateGoogleGears = function(callback, errorCallback) {
        var geo = google.gears.factory.create('beta.geolocation');
        geo.getCurrentPosition(function(position) {
            initialLocation = new google.maps.LatLng(position.latitude, position.longitude);
            callback(initialLocation);
        }, function(error) {
            handleNoGeoLocation(browserSupportFlag);
            errorCallback(error);
        });
    };

    var handleNoGeolocation = function(errorFlag) {
        if (errorFlag == true) {
            //Geolocation service failed.
        } else {
            //Your browser doesn't support geolocation.
        }
        map.setCenter(initialLocation);
    };
    return {
        Initialize: initialize,
        Locate: locate,
        GetLastLocation: getLastLocation
    };
}(jQuery);