YouMap = { };

//region GLOBAL

$(document).ready(function () {
    VK.init({
        apiId: 2831071
    });
});

var getMap = function() {
    return $("#map").data("GoogleMap");
};

var createMarker = function (config, onDrag) {
    var map = getMap();
    if (!map.Markers) {
        map.Markers = new Array();
    }
    var marker = new $.telerik.GoogleMarker(map.GMap, map.Markers.length, config);

    map.Markers.push(marker);
    map.renderMarker(marker);
    google.maps.event.addListener(marker.GMarker, 'drag', function (e) {
        $('#Latitude').val(e.latLng.Sa);
        $('#Longitude').val(e.latLng.Ta);
    });
    return marker;
};


var updatePosition = function (marker, location) {
    marker.GMarker.setPosition(location);
};

//endregion GLOBAL

YouMap.Map = function ($) {

    var closeZoom = 18;
    var geocoder = null;
    var userLocation = null;
    var userMarker = null;
    
    var initialize = function () {

        $(".place-info-window").each(function() {
            var id = "vk_like_place_" + $(this).data("id");
            var baseUri = $(this).baseURI;
            var url = baseUri + $("a", $(this)).attr("href");
            var title = $(".place-content h2", $(this)).html();
            var desc = $(".place-content p", $(this)).html();
            var image = baseUri + $(".place-logo img", $(this)).attr("src");

            VK.Widgets.Like(id, {
                pageUrl: url,
                pageImage: image,
                pageTitle: title,
                pageDescription: desc,
                width: 100
            });
        });

        YouMap.Geolocation.Locate(function(location) {          
            setMapCenter(location.Sa, location.Ta);
            updateUserMarker(location);
        });
        startUpdateLocation();
        //$("#map").attr("style", "height: 213px; position: relative; background-color: rgb(229, 227, 223); overflow: initial;");
        $("#map").css("width", "auto");

        setMapHeight();
        $(window).resize(function(e) {
            setMapHeight();
        });
        
        if (geocoder == null) {
            geocoder = new google.maps.Geocoder();
        }
        YouMap.Vk.Map.Initialize();
    };
    
    var updateUserMarker = function (location) {
        userLocation = location;
        if (userMarker) {
            userMarker.Latitude = location.Sa;
            userMarker.Longitude = location.Ta;
            Request.get("/Map/CheckNearby").addParams({
                Latitude: location.Sa,
                Longitude: location.Sa
            });
        } else {
            userMarker = createMarker({
                Latitude: location.Sa,
                Longitude: location.Ta,
                Title: "Я"
            });
        }
    };

    var startUpdateLocation = function() {
        setTimeout(function() {
            YouMap.Geolocation.Locate(updateUserMarker);
            startUpdateLocation();
        },300000);
    };

    //вычисление значения Zoom по границам
    var getZoom = function (bounds) {

        var width = $("#map").width();
        var height = $("#map").height();

        var dlat = Math.abs(bounds.getNorthEast().lat() - bounds.getSouthWest().lat());
        var dlon = Math.abs(bounds.getNorthEast().lng() - bounds.getSouthWest().lng());

        var max = 0;
        if (dlat > dlon) {
            max = dlat;
        } else {
            max = dlon;
        }

        // Center latitude in radians
        var clat = Math.PI * Math.abs(bounds.getSouthWest().lat() + bounds.getNorthEast().lat()) / 360.;

        var C = 0.0000107288;
        var z0 = Math.ceil(Math.log(dlat / (C * height)) / Math.LN2);
        var z1 = Math.ceil(Math.log(dlon / (C * width * Math.cos(clat))) / Math.LN2);

        return 18 - ((z1 > z0) ? z1 : z0);
    };

    var createLocation = function (x, y) {
        return new google.maps.LatLng(x, y);
    };

    var setMapCenter = function (x, y) {
        var map = getMap();
        map.GMap.setCenter(createLocation(x, y));
        map.GMap.setZoom(closeZoom);
    };

    var searchGoogle = function(address,callback) {
        var map = getMap();

        geocoder.geocode({ 'address': address }, function(results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                var location = results[0].geometry.location;
                map.GMap.setCenter(location);
                map.GMap.setZoom(getZoom(results[0].geometry.viewport));
                callback(location);

            } else {
                alert("Пошло что-то не так, потому что: " + status);
            }
        });
    };

    var setMapHeight = function()
    {
        var height = $(window).height() - $("header").height();
        if ($("#controlPanel").length > 0) {
            $("#map").css("height", height - $("#controlPanel").height());
        } else {
            $("#map").css("height", height);
        }
    };

    return {
        Initialize: initialize,
        SetMapCenter: setMapCenter,
        SetMapHeight: setMapHeight,
        SearchGoogle: searchGoogle
    };
} (jQuery);

YouMap.ControlPanel = function ($) {

    var initialize = function () {
    };

    return {
        Initialize: initialize
    };
} (jQuery);


YouMap.AddPlace = function ($) {
    var initialize = function () {
        $("#search").click(function (event) {
            setMapToCity();
            return false;
        });

        $("#Address").keydown(function (event) {
            if (event.which == 13) {
                setMapToCity();
                return false;
            }
        });

        $('#selectIconBtn').click(function () {
            $.colorbox({ href: $(this).attr("href"), width: 600, height: 400 });
            return false;
        });

        $('.icon-select').live("click", function () {
            var src = $(this).find("img").attr("src");
            $("#Icon").val(src);
            $("#currentIcon").attr("src", src.replace("64x64", "24x24"));
            $.colorbox.close();
        });

       
    };
    
    var setMapToCity = function () {
        var address = $("#Address").val();
        YouMap.Map.SearchGoogle(address,function(location) {
            setMarkerPosition(location);
            updateFormFields(location);
        });
        
    };

    var marker = null;

    var setMarkerPosition = function (location) {

        if (marker) {
            updatePosition(marker, location);
        } else {
            marker = createMarker({
                Latitude: location.Sa,
                Longitude: location.Ta,
                Draggable: true,
                Title: "New marker"
            }, markerDragged);
        }

    };

 

    var markerDragged = function (parameters) {
        updateFormFields(parameters);
    };

    var updateFormFields = function (location) {
        $('#Latitude').val(location.Sa);
        $('#Longitude').val(location.Ta);
    };

    
    return {
        Initialize: initialize
    };
} (jQuery);

