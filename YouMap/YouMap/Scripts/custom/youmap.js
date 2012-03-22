YouMap = { };

var getMap = function() {
    return YouMap.Map.GetMap();
};
//endregion GLOBAL

YouMap.Map = function ($) {

    var closeZoom = 17;
    var geocoder = null;
    var userLocation = null;
    var userMarker = null;
    var userProfileUrl = "/Account/Profile";
    var city = "Минск";
    var country = "Беларусь";

    var map = null;
    var places = new Array();

    var initialize = function (config,placesconfigs) {
        places = placesconfigs;
        map = YouMap.Google.CreateMap(config);
        for (var i = 0; i < places.length; i++) {
            places[i].click = openPlaceInfo;
            var marker = YouMap.Google.CreateMarker(map, places[i]);    
            YouMap.Google.AddMarker(map, marker);
            places[i].Marker = marker;
            if (places[i].OpenOnLoad) {
                navigateToPlace(places[i]);
            }
        }
        
        setTimeout(function() {
            YouMap.Geolocation.Locate(function (x, y) {
                setMapCenter(x, y);
                updateUserMarker(x, y);
            });
        }, 100);

       
        startUpdateLocation();
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
        $.get(options.InfoWindowUrl, function (result) {
            YouMap.Google.OpenWindow(map, marker, result);
            setTimeout(function() {
               
                var id = "vk_like_place_" + temp.Id;
                var placeInfoWindow = $("#" + id).find(".place-info");
                var baseUri = window.location.origin;
                var url = baseUri + options.InfoWindowUrl;
                var title = $("h2", placeInfoWindow).html();
                var desc = $("p", placeInfoWindow).html();
                var image = baseUri + $("img", placeInfoWindow).attr("src");

                var r = VK.Widgets.Like(id, {
                    pageUrl: url,
                    pageImage: image,
                    type: 'button',
                    pageTitle: title,
                    pageDescription: desc,
                    width: 100
                });
                if (r) {
                    $("#" + id).hide();
                }
                $(".accordion").accordion({
                    header: '.accordion-heading'
                }
                );
            },10);
        });
    };


    var navigateToPlace = function(place) {
        setMapCenter(place.X, place.Y);
        openPlaceInfo(place);
    };

    var navigateToPlaceById = function(id) {
        for (var i = 0; i < places.length; i++) {
            if (places[i].Id == id) {
                navigateToPlace(places[i]);
            }
        }
    };

    var openUserInfo = function() {
        $.get(userProfileUrl, function (content) {

            YouMap.Google.OpenWindow(getMap(), userMarker, content);
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
                Title: "Я",
                click: openUserInfo
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

    var filterPlaces = function(filter) {
        for (var i = 0; i < places.length; i++) {
            var place = places[i];
            if (filter.categories ) {
                if (filter.categories.length > 0 && filter.categories.indexOf(place.CategoryId) == -1) {
                    YouMap.Google.RemoveMarker(place.Marker);
                } else {
                    YouMap.Google.AddMarker(map, place.Marker);
                }
            }
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
        },
        FilterPlaces: filterPlaces,
        NavigateToPlaceById: navigateToPlaceById
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
            marker = YouMap.Google.CreateMarker(getMap(),{
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
        $("#PrivateTrue, #PrivateFalse").change(function() {
            if ($(this).val() == "True") {
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
            var message = $("#placeFields #Title").val();
            if ($("#placeFields #PlaceTitle").val()) {
                message += ".\n Встреча в \"" + $("#placeFields #PlaceTitle").val() + "\"";
            }
            if ($("#placeFields input[name='UserIds']").length > 0) {
                var hiddens = $("#placeFields input[name='UserIds']");
                var users = new Array();               
                for (var i = 0; i < hiddens.length; i++) {
                    users.push("*id" + $(hiddens[i]).val());
                }
                message += ".\n Участники: " + users.join(", ");
            }
            message += ".\n" + $("#placeFields #Start").val() + " " + $("#placeFields #Hour").val() + ":" + $("#placeFields #Minute").val();
            if ($("#placeFields #Memo").val()) {
                message += ".\n" + $("#placeFields #Memo").val();
            }
            VK.Api.call("wall.post", { message: message, attachments: window.location.origin + data.jsonItems.url }, function() {
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


    var addFriend = function(friend) {
        var span = $("<li/>")
                            .append(friend.first_name + " " + friend.last_name + " (<a>x</a>)")
                            .append("<input type='hidden' name='UserIds' value='" + friend.uid + "'/>");
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