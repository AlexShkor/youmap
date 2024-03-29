﻿YouMap = { };

var getMap = function() {
    return YouMap.Map.GetMap();
};
//endregion GLOBAL

YouMap.Map = function ($) {

    var closeZoom = 17;
    var geocoder = null;
    var userLocation = null;
    var userMarker = null;
    var userIcon = null;
    var userProfileUrl = "/Account/Profile";
    var city = "Минск";
    var country = "Беларусь";
    var isCustomUserLocation = false;
    var map = null;
    var places = new Array();

    var initialize = function (config, mapUrl) {
        map = YouMap.Google.CreateMap(config,zoomCallback);
        Request.get(mapUrl || window.location.href).addSuccess("markersload", function (data) {
            places = data.jsonItems.model.Places;
            for (var i = 0; i < places.length; i++) {
                places[i].click = openPlaceInfo;
                var marker = YouMap.Google.CreateMarker(places[i]);
                YouMap.Google.AddMarker(marker);
                places[i].Marker = marker;
                places[i].VisibleByFilter = true;
                if (places[i].OpenOnLoad) {
                    navigateToPlace(places[i], config.DisableAutoOpenPlaceInfo);
                }
            }
            if (config.OpenPopupUrl) {
                colorbox(config.OpenPopupUrl);
            }
        }).send();
        userIcon = config.UserIcon;
        if (config.UserLocation) {  
            setMapCenter(config.UserLocation.Latitude, config.UserLocation.Longitude);
            updateUserMarker(config.UserLocation.Latitude, config.UserLocation.Longitude);
            isCustomUserLocation = true;
        } else {
            setMapCenter(config.Defaultlocation.Latitude, config.Defaultlocation.Longitude);
            updateUserMarker(config.Defaultlocation.Latitude, config.Defaultlocation.Longitude);
            setTimeout(function() {
                YouMap.Geolocation.Locate(function(x, y) {
                    setMapCenter(x, y);
                    updateUserMarker(x, y);
                    submitUserLocation();
                });
            }, 100);
            startUpdateLocation();
        }
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
        var temp = options;
        var marker = options.Marker;
        if (options.OpenInNewWindow) {
            window.location = options.InfoWindowUrl;
            return;
        }
        $.get(options.InfoWindowUrl, function (result) {
            YouMap.Google.OpenWindow(marker, result);
            drawPlaceVkLike(temp.Id, options.Title, $(result).find("img").attr("src"));
            drawAccordion();         
        });
    };

    var drawPlaceVkLike = function (placeId, title,image) {
        setTimeout(function() {
            var id = "vk_like_place_" + placeId;
            var baseUri = window.location.origin;
            var url = baseUri + "/?placeId=" + placeId;
            var desc = "На YouMap.BY.";
            if (VK) {
                VK.Widgets.Like(id, {
                    pageUrl: url,
                    pageImage: baseUri + image,
                    type: 'mini',
                    pageTitle: title,
                    pageDescription: desc,
                    width: 100
                });
            }
        }, 0);       
    };

    var drawAccordion = function () {
        if ($(".accordion").length == 0) {
            setTimeout(drawAccordion, 100);
            return;
        }
        $(".accordion").parent("div").css("overflow", "hidden");
        $(".accordion").accordion({
            header: '.accordion-heading'
        });
    };

    var navigateToPlace = function(place,dontOpen) {
        setMapCenter(place.X, place.Y);
        if (dontOpen) {
            return;
        }
        openPlaceInfo(place);
    };

    var navigateToPlaceById = function(id) {
        for (var i = 0; i < places.length; i++) {
            if (places[i].Id == id) {
                navigateToPlace(places[i]);
            }
        }
    };
    
    var layer;
    var zoomCallback = function () {
        layer = (15 - map.zoom);
        if (layer < 0) {
            layer = 0;
        }
        if (layer> 5) {
            layer = 5;
        }
        setTimeout(function() {
            if (layer != currentLayer) {
                setLayer(layer);
            }
        }, 100);
        
    };
    
    var currentLayer = 999;

    var setLayer = function(layer) {
        for (var i = 0; i < places.length; i++) {
            var place = places[i];
            if (place.Layer < layer) {
                hidePlaceWithLayer(place, layer);
            } else {
                showPlaceWithLayer(place, layer);
            }
        }
        currentLayer = layer;
    };

    var hidePlaceWithLayer = function(place, layer) {
        place.VisibleByLayer = false;
        updatePlace(place);
    };
    
    var showPlaceWithLayer = function(place, layer) {
        place.VisibleByLayer = true;
        updatePlace(place);
    };

    var openUserInfo = function() {
        $.get(userProfileUrl, function (content) {

            YouMap.Google.OpenWindow(userMarker, content);
        });
    };

    var updateUserMarker = function (x, y) {
        userLocation = {x: x, y: y};
        if (userMarker) {
            YouMap.Google.SetPosition(userMarker, x, y);
        } else {
            userMarker = YouMap.Google.CreateMarker({
                X: x,
                Y: y,
                Icon: userIcon,
                Title: "Вы здесь",
                click: openUserInfo
            });
        }
    };

    var userDragging = false;

    var toggleUserDrag = function () {
        var x = userMarker.position.lat();
        var y = userMarker.position.lng();
        userLocation = { x: x, y: y };
        YouMap.Google.RemoveMarker(userMarker);
        userMarker = YouMap.Google.CreateMarker({
            X: x,
            Y: y,
            Icon: userIcon,
            Title: "Вы здесь",
            click: userDragging? openFinishDrag : openUserInfo,
            Draggable: !userDragging
        });
        userDragging = !userDragging;
        if (!userDragging) {
            submitUserLocation();
        }
    };

    var openFinishDrag = function() {
        var anchor = $("<a href='#' class='btn btn-mini btn-danger>Закрепить маркер<a/>");
        anchor.click(function() {
            toggleUserDrag();
            return false;
        });
        YouMap.Google.OpenWindow(userMarker, anchor);
    };


    var getUserLocation = function() {
        return userLocation;
    };

    var locateUser = function() {
        YouMap.Geolocation.Locate(updateUserMarker);
    };
   
    var startUpdateLocation = function() {
        setTimeout(function() {
            locateUser();
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
        if (geocoder == null) {
            geocoder = new google.maps.Geocoder
        }
        geocoder.geocode({ 'address': address }, function(results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                var location = results[0].geometry.location;
                if (map) {
                    map.setCenter(location);
                    map.setZoom(getZoom(results[0].geometry.viewport));
                }
                callback(location.lat(),location.lng());

            } else {
                alert("Пошло что-то не так, потому что: " + status);
            }
        });
    };

    var updateUserLocation = function (callback) {       
            YouMap.Geolocation.Locate(function(x, y) {
                userLocation = { x: x, y: y };
                Request.post("/Vk/SubmitLocation").addParams({
                    X: x,
                    Y: y
                }).send();
                callback(x, y);
            });
    };

    var searchByLocation = function(x, y, callback) {
        var location = new google.maps.LatLng(x, y);
        if (geocoder == null) {
            geocoder = new google.maps.Geocoder();
        }
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

    var filterPlaces = function(filter) {
        for (var i = 0; i < places.length; i++) {
            var place = places[i];
            if (filter.categories ) {
                if (filter.categories.length > 0 && filter.categories.indexOf(place.CategoryId) == -1) {
                    place.VisibleByFilter = false;
                    
                } else {
                    place.VisibleByFilter = true;
                }
                updatePlace(place);
            }
        }
    };

    var updatePlace = function (place) {
        if ((place.VisibleByFilter && place.VisibleByLayer) || place.VisibleBySearch) {
            if (place.Marker.getMap()) {
                return;
            } else {
                YouMap.Google.AddMarker(place.Marker);
            }
        } else {
            if (place.Marker.getMap()) {
                YouMap.Google.RemoveMarker(place.Marker);
            } else {
                return;
            }       
        }
    };

    var submitUserLocation = function() {
        Request.post("/Vk/SubmitLocation").addParams({
            X: userLocation.x,
            Y: userLocation.y
        }).send();
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
        },
        SetUserLocation: updateUserMarker,
        SubmitUserLocation: submitUserLocation,
        FilterPlaces: filterPlaces,
        NavigateToPlaceById: navigateToPlaceById,
        ToggleUserDrag: toggleUserDrag,
        UpdateUserLocation: updateUserLocation
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
        $("#tagit").tagit({ select: true });
       
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
                X: x,
                Y: y,
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

YouMap.AddEvent = function($) {

    var friends;
    
    var initialize = function () {
        $("#addFriends").click(function () {
            
            if (!$(".friends-select").parent().is(":visible")) {
                showFriendsList();
                $("#friendsSelect").focus();
            } else {
                hideFriendsList();
            }
        });
        $("#Start").datepicker({ dateFormat: "dd.mm.yy" });
        loadFriendsList();
        $(".friends-select").on("click", "a", function() {
            $(this).parent().remove();
            $.colorbox.resize();
        });
        $("#submitEvent").bind("success", function (event, data) {
            var model = data.jsonItems.model;
            var message = YouMap.Vk.Map.CreateEventShareMessage(model);
            VK.Api.call("wall.post", { message: message, attachments: window.location.origin + model.ShareUrl }, function () {
            });
        });
    };


    var showFriendsList = function() {
        if (!friends) {
            loadFriendsList();
        }
        
        $(".friends-select").parent().show("slow", function() {
            $.colorbox.resize();
        });
    };


    var addFriend = function (friend) {
        var fullName = friend.first_name + " " + friend.last_name;
        var span = $("<li/>")
                            .append(fullName + " (<a>x</a>)")
                            .append("<input type='hidden' name='UserIds' value='" + friend.uid + "'/>")
                            .append("<input type='hidden' name='UserNames' value='" + fullName + "'/>");
        span.data("photo", friend.photo);
        span.appendTo(".friends-select");
    };

    var hideFriendsList = function() {
        $(".friends-select").parent().hide("slow", function() {
            $.colorbox.resize();
        });
        
    };

    var loadFriendsList = function() {
        VK.Api.call('friends.get', { fields: "uid,first_name,last_name,photo", order: "hints" }, function(r) {
            friends = r.response;

            $("#friendsSelect").autocomplete({
                minLength: 1,
                source: function(request,response) {
                    var items = new Array();
                    for (var i = 0; i < friends.length; i++) {
                        if (items.length == 10) {
                            return;
                        }
                        var item = friends[i];
                        if ((item.first_name + " " + item.last_name).toLowerCase().indexOf(request.term.toLowerCase()) != -1){
                             items.push(item);
                         }
                    }
                    response(items);
                },
                select: function (event, ui) {
                    if ($(".friends-select input[value='" + ui.item.uid + "']").length == 0) {
                        addFriend(ui.item);
                        $.colorbox.resize();
                    }
                    $("#friendsSelect").val("");
                    return false;
                }
            })
            .data("autocomplete")._renderItem = function (ul, item) {
                return $("<li class='friend'></li>")
				.data("item.autocomplete", item)
	            .append("<a><img src='" + item.photo + "'/>"+ item.first_name + " " + item.last_name +"</a>")
				.appendTo(ul);
            };
        });
    };

    return {
        Initialize: initialize
    };
}(jQuery);