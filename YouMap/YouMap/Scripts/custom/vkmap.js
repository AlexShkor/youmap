YouMap.Vk = {   };

YouMap.Vk.Panel = function($) {

    var initialize = function(isVkUser) {
        if(isVkUser) {
            $("#showSettings").click(function () {
                $("#vkPanel .container").slideToggle("fast");
            });
            $("input[name='ShowFriends']").live('click',function() {
                if ($(this).is(':checked')) {
                    YouMap.Vk.Map.ShowFriends();
                } else {
                    YouMap.Vk.Map.HideFriends();
                }
            });
        }
    };

    var userView = function() {
        $("#showSmallLogon").click(function () {
            $("#loginPanel .container").slideToggle("fast");
        });

        $("#profilePanel .actions a").click(function() {
            $("#profilePanel .container").slideToggle("fast");
        });
        $("#logoutLink").click(function() {
            VK.Auth.logout();
        });
    };

    var controlPanel = function () {
        $("#showControls").click(function () {
            $("#topControl .container").slideToggle("fast");
        });
    };

    var checkInInit = function () {
        if (!$("#checkin #PlaceId").val()) {
            var loc = YouMap.Map.GetUserLocation();
            YouMap.Map.SearchByLocation(loc.x, loc.y, function (result) {
                $("#checkin textarea").html(result[0].formatted_address);
            });
        }
        var relativeUrl = null;
        $(".checkin .ajax-submit").click(function() {
            relativeUrl = $("#checkin #CheckInUrl").val();
            if (!relativeUrl) {
                var location = YouMap.Map.GetUserLocation();
                $(".checkin #Latitude").val(location.x);
                $(".checkin #Longitude").val(location.y);
                relativeUrl = "/?latitude=" + location.x + "&longitude=" + location.y;
            }
        });
        $(".checkin .ajax-submit").bind("success",function () {
                VK.Api.call("wall.post", { message: $(".checkin textarea").val(), attachments: window.location.origin + relativeUrl}, function() {
            });
        });
    };



    return {
        Initialize: initialize,
        UserView: userView,
        CheckInInit: checkInInit,
        ControlPanel: controlPanel
    };
}(jQuery);

YouMap.Vk.Map = function($) {

    var friends = null;
    var tempMarkers = new Array();
    
    var initialize = function() {
        VK.Observer.subscribe("auth.login", getFriends);
        VK.Observer.subscribe("auth.logout", hideFriendsMarkers);
        VK.Auth.getLoginStatus(function (response) {
            if (response.session) {
                getFriends();
            }
        });
        userInfo();
    };

    var getFriends = function() {
        VK.Api.call('friends.get', { fields: "uid,first_name,last_name,photo" }, function (r) {
            if (r.response) {
                if (friends) {
                    return;
                }
                friends = r.response;

                var ids = friends.map(function(item) {
                    return item.uid;
                }).join(",");
                Request.post("/Vk/GetUsersLocation").addParams({ ids: ids }).
                    addSuccess("getuserslocations", function (data) {
                        for (var i = 0; i < data.jsonItems.model.length; i++) {
                            for (var j = 0; j < friends.length; j++) {
                                var item = data.jsonItems.model[i];
                                var friend = friends[j];
                                if (item.Id == friend.uid) {
                                    createFriendMarker(friend,item );
                                }
                            }
                        }
                    
                }).send();
            }
        });
    };

    var createFriendMarker = function (friend, item) {
        var options = {
            Id: friend.Uid,
            X: item.X,
            Y: item.Y,
            InfoWindowUrl: item.InfoWindowUrl,
            Title: friend.first_name + " " + friend.last_name,
            Icon: {
                Path: friend.photo,
                Size: {
                    Width: 50,
                    Height: 50,
                    IsEmpty: false
                },
                Point: { X: 0, Y: 0, IsEmpty: true },
                Anchor: { X: -7, Y: 57, IsEmpty: false }
            },
            Shadow: item.Shadow,
            click: openInfo
        };
        var map = getMap();
        options.marker = YouMap.Google.CreateMarker(map, options);
        friend.options = options;
    };
    
    var openInfo = function(options) {
        $.get(options.InfoWindowUrl, function(content) {
            YouMap.Google.OpenWindow(getMap(), options.marker, content);
        });
    };

    var hideFriendsMarkers = function() {
        for (var i in friends) {
            friends[i].options.marker.setMap(null);
        }
    };
    var showFriendsMarkers = function () {
        var map = getMap();
        for (var i in friends) {
            friends[i].options.marker.setMap(map);
        }
    };

    var lastTempMarkersUrl = null;
    var userInfo = function () {
        $(".showEvents, .showCheckins").live("click", function () {
            $(this).toggleClass("toggle");
            var url = $(this).attr("href");
            if (url == lastTempMarkersUrl) {
                hideMarkers(tempMarkers);
                lastTempMarkersUrl = null;
            } else {
                lastTempMarkersUrl = url;
                Request.get(url).addSuccess("onLoaded", function (data) {
                    hideMarkers(tempMarkers);
                    tempMarkers = data.jsonItems.model;
                    for (var i = 0; i < tempMarkers.length; i++) {
                        var options = tempMarkers[i];
                        createMarker(options);
                    }
                }).send();
            }
            return false;
        });
    };

    var hideMarkers = function (markers) {
        for (var i in markers) {
            markers[i].marker.setMap(null);
        }
    };

    var createMarker = function(options) {
        options.click = openInfo;
        options.marker = YouMap.Google.CreateMarker(getMap(), options);
        YouMap.Google.AddMarker(getMap(),options.marker);
    };

    return {
        Initialize: initialize,
        ShowFriends: showFriendsMarkers,
        HideFriends: hideFriendsMarkers,
        UserInfo: userInfo
    };
}(jQuery);