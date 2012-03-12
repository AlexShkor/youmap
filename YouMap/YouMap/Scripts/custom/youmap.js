YouMap = { };

//region GLOBAL

$(document).ready(function () {
    VK.init({
        apiId: 2831071
    });
});

var getMap = function() {
    return YouMap.Map.GetMap();
};
//endregion GLOBAL

YouMap.Map = function ($) {

    var closeZoom = 18;
    var geocoder = null;
    var userLocation = null;
    var userMarker = null;

    var city = "Минск";
    var country = "Беларусь";

    var map = null;
    var places = new Array();

    var initialize = function (config, placeOptions) {
        places = placeOptions;
        map = YouMap.Google.CreateMap(config);
        for (var i = 0; i < places.length; i++) {
            places[i].click = openPlaceInfo;
            var marker = YouMap.Google.CreateMarker(map, places[i]);    
            YouMap.Google.AddMarker(map, marker);
            places[i].Marker = marker;
        }

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


        setTimeout(function() {
            YouMap.Geolocation.Locate(function (x, y) {
                setMapCenter(x, y);
                updateUserMarker(x, y);
            });
        }, 100);

       
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

    var openPlaceInfo = function(options) {
        var marker = options.Marker;
        $.get(options.InfoWindowUrl, function (result) {
            YouMap.Google.OpenWindow(map, marker, result);
        });
    };

    var updateUserMarker = function (x,y,dontCheckNearby) {
        userLocation = {x: x, y: y};
        if (userMarker) {
            YouMap.Google.SetPosition(userMarker, x, y);
        } else {
            userMarker = YouMap.Google.CreateMarker(map,{
                X: x,
                Y: y,
                Title: "Я"
            });
        }
        if (!dontCheckNearby) {
            Request.get("/Map/CheckNearby").addParams({
                Latitude: x,
                Longitude: y
            }).send();
        }
    };

    var getUserLocation = function() {
        return userLocation;
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
        map.setCenter(createLocation(x, y));
        map.setZoom(closeZoom);
    };

    var searchGoogle = function(address,callback) {
        if (address.indexOf(city) == -1) {
            address += ", " + city;
        }
        if (address.indexOf(country) == -1) {
            address += ", " + country;
        }
        geocoder.geocode({ 'address': address }, function(results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                var location = results[0].geometry.location;
                map.setCenter(location);
                map.setZoom(getZoom(results[0].geometry.viewport));
                callback(location.Ua,location.Va);

            } else {
                alert("Пошло что-то не так, потому что: " + status);
            }
        });
    };
    


    var searchByLocation = function(x, y, callback) {
        var location = new google.maps.LatLng(x, y);
        geocoder.geocode({ location: location }, function(result, status) {
            callback(result);
        });
    };

    var setMapHeight = function()
    {
        var height = $(window).height() - $("header").height() - 20;
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
        SearchGoogle: searchGoogle,
        GetUserLocation: getUserLocation,
        SearchByLocation: searchByLocation,
        GetMap: function () {
            return map;
        }
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
        $("#Address").focusout(setMarkerPosition);
       
    };
    
    var setMapToCity = function () {
        var address = $("#Address").val();
        YouMap.Map.SearchGoogle(address,function(x,y) {
            setMarkerPosition(x,y);
            updateFormFields(x,y);
        });
        
    };

    var marker = null;

    var setMarkerPosition = function (x,y) {

        if (marker) {
            YouMap.Google.SetPosition(marker, x, y);
        } else {
            marker = YouMap.Google.CreateMarker({
                Latitude: x,
                Longitude: y,
                Draggable: true,
                Title: "New marker",
                drag: markerDragged
            });
        }

    };

 

    var markerDragged = function (x,y) {
        updateFormFields(x,y);
    };

    var updateFormFields = function (x,y) {
        $('#addPlaceContainer #Latitude').val(x);
        $('#addPlaceContainer #Longitude').val(y);
    };

    
    return {
        Initialize: initialize
    };
} (jQuery);

