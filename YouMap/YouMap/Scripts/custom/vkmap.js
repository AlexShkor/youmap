YouMap.Vk = {};

YouMap.Vk.Map = function($) {

    var friends = null;
    var friendsMarkers = null;
    
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
                        friendsMarkers = new Array();
                        for (var i = 0; i < data.jsonItems.locations.length; i++) {
                            for (var j = 0; j < friends.length; j++) {
                                var item = data.jsonItems.locations[i];
                                var friend = friends[j];
                                if (item.Id == friend.uid) {
                                    createFriendMarker(friend, item.Latitude, item.Longitude);
                                }
                            }
                        }
                    
                }).send();
            }
        });
    };

    var createFriendMarker = function(friend, x, y) {
        var marker = createMarker({
            Latitude: x,
            Longitude: y,
            Title: friend.first_name + " " + friend.last_name,
            Icon: {
                Path: friend.photo,
                Size: {
                    Width: 50, Height: 50, IsEmpty: false
                },
                Point: {X: 0, Y: 0, IsEmpty: true},
                Anchor: { X: 25, Y: 50, IsEmpty: false }
            }
        });
        friendsMarkers.push(marker);
        //marker.uid = friend.uid;
    };


    var hideFriendsMarkers = function() {

    };

    return {
        Initialize: initialize
    };
}(jQuery);