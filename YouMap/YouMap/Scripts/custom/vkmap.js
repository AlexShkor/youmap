YouMap.Vk = {   };

YouMap.Vk.Panel = function($) {

    var initialize = function(isVkUser) {
        if(isVkUser) {
            $("#showSettings").click(function () {
                $("#vkPanel .container").slideToggle("fast");
            });
            $("#ShowFriends").click(function() {
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
        $(".checkin .ajax-submit").click(function() {
            var location = YouMap.Map.GetUserLocation();
            $(".checkin #Latitude").val(location.x);
            $(".checkin #Longitude").val(location.y);

            VK.Api.call("wall.post", { message: $(".checkin textarea").val(), attachments: window.location.origin + "/#:" + location.x + ":" + location.y }, function() {

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
    
    var initialize = function() {
        VK.Observer.subscribe("auth.login", getFriends);
        VK.Observer.subscribe("auth.logout", hideFriendsMarkers);
        VK.Auth.getLoginStatus(function (response) {
            if (response.session) {
                getFriends();
            }
        });
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
                        for (var i = 0; i < data.jsonItems.locations.length; i++) {
                            for (var j = 0; j < friends.length; j++) {
                                var item = data.jsonItems.locations[i];
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
            X: item.Latitude,
            Y: item.Longitude,
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
            Shadow: {
                Path: window.location.origin + "/UserFiles/border.png",
                Size: {
                    Width: 60,
                    Height: 60,
                    IsEmpty: false
                },
                Point: { X: 0, Y: 0, IsEmpty: true },
                Anchor: { X: 0, Y: 60, IsEmpty: false }
            }
        };
        var map = getMap();
        friend.marker = YouMap.Google.CreateMarker(map,options);
    };


    var hideFriendsMarkers = function() {
        for (var i in friends) {
            friendsMarkers[i].marker.setMap(null);
        }
    };
    var showFriendsMarkers = function () {
        var map = getMap();
        for (var i in friends) {
            friends[i].marker.setMap(map);
        }
    };
    return {
        Initialize: initialize,
        ShowFriends: showFriendsMarkers,
        HideFriends: hideFriendsMarkers
    };
}(jQuery);