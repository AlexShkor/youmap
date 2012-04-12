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
        $("#logoutLink").live("click", function () {
            if (VK)
                VK.Auth.logout();
        });
        $("#openChangeLocationBox").live("click", function () {
            $(this).parent().find("#changeLocationBox").slideToggle("fast");
            $(this).parent().prev().slideToggle("fast");
        });
        $("#toggleDrag").live("click", function() {
            $(this).toggleClass("btn-primary");
            $(this).toggleClass("btn-danger");
            var text = $(this).html();
            $(this).html($(this).data("alttext"));
            $(this).data("alttext", text);
            YouMap.Map.ToggleUserDrag();
        });
        $("#dragToAddress").live("click", function () {
            YouMap.Map.SearchGoogle($(this).parents(".well").find("input").val(), function(x, y) {
                YouMap.Map.SetUserLocation(x, y);
                YouMap.Map.SubmitUserLocation();
            });
        });
    };

    var controlPanel = function () {
        $("#showControls").click(function () {
            $("#topControl .container").slideToggle("fast");
        });
    };

    var checkInInit = function (located) {
        //if (!located) {
        //    YouMap.Geolocation.Locate(function(x, y) {
        //        Request.get("/Places/Near").addParams({
        //            x: x,
        //            y: y
        //        }).send();
        //    });
        //}
        if (!$("#checkin #PlaceId").val()) {
            var loc = YouMap.Map.GetUserLocation();
            if (loc) {
                YouMap.Map.SearchByLocation(loc.x, loc.y, function(result) {
                    $("#checkin textarea").html(result[0].formatted_address);
                });
            }
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
        $(".checkin .ajax-submit").bind("success", function () {
            if (VK)
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
    
    var initialize = function () {
        if (VK) {
            VK.Observer.subscribe("auth.login", getFriends);
            VK.Observer.subscribe("auth.logout", hideFriendsMarkers);
            VK.Auth.getLoginStatus(function(response) {
                if (response.session) {
                    getFriends();
                }
            });
            startFriendsTracking();
        }
        userInfo();
    };

    var startFriendsTracking = function() {
        setTimeout(function() {
            getFriends();
            startFriendsTracking();
        },30000);
    };

    var getFriends = function () {
        if (VK)
        VK.Api.call('friends.get', { fields: "uid,first_name,last_name,photo" }, function (r) {
            if (r.response) {
                if (friends || r.response.length == 0) {
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
        options.marker = YouMap.Google.CreateMarker(options);
        friend.options = options;
    };
    
    var openInfo = function(options) {
        $.get(options.InfoWindowUrl, function(content) {
            YouMap.Google.OpenWindow(options.marker, content);
        });
    };

    var hideFriendsMarkers = function() {
        for (var i in friends) {
            var friend = friends[i];
            if (friend.options && friend.options.marker)
                YouMap.Google.AddMarker(friend.options.marker);
        }
    };
    var showFriendsMarkers = function () {
        for (var i in friends) {
            var friend = friends[i];
            if (friend.options && friend.options.marker)
                YouMap.Google.AddMarker(friend.options.marker);
        }
    };

    //TODO: need to be separated into two methods, for events and checkins
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
        options.click = openContent;
        options.marker = YouMap.Google.CreateMarker(options);
        YouMap.Google.AddMarker(options.marker);  
    };


    var loadFriendsLiks = function(html, callback) {
        var anchors = html.find(".users a");
        var uids = jQuery.map(anchors, function (n, i) {
            return $(n).data("uid");
        });
        if (VK)
            VK.Api.call("users.get", { uids: uids.join(","), fields: "uid, first_name, last_name" }, function (result) {
                if (result && result.response) {
                    for (var i = 0; i < result.response.length; i++) {
                        var user = result.response[i];
                        html.find("#linkFor" + user.uid).html(user.first_name + " " + user.last_name);
                    }
                }
                if (callback) {
                    callback(html);
                }
            });
    };

    //TODO: need to be separated into two methods, for events and checkins
    var openContent = function (options) {
        var html = $(options.Content);
        loadFriendsLiks(html, function(elem) {
            YouMap.Google.OpenWindow(options.marker, $("<div/>").append(elem).html());
        });
    };

    var initEventDetails = function(model) {
        loadFriendsLiks($(".event-details"));      
        if (VK) {
            VK.Widgets.Like("eventVkLike", {
                pageUrl: model.ShareUrl,
                type: 'mini',
                pageTitle: model.Title,
                pageDescription: createEventShareMessage(model),
                width: 100
            });
        }
    };
    
    var createEventShareMessage = function (model) {
        var message = model.Title;
        if (model.PlaceTitle) {
            message += ".\n Встреча в \"" + model.PlaceTitle + "\"";
        }
        if (model.UsersIds && model.UsersIds.length > 0) {
            var users = new Array();
            users.push("*id" + model.OwnerVkId);
            for (var i = 0; i < model.UsersIds.length; i++) {
                users.push("*id" + model.UsersIds[i]);
            }
            message += ".\n Участники: " + users.join(", ");
        }
        message += ".\n" + model.StartDate;
        if (model.Memo) {
            message += ".\n" + model.Memo;
        }
        return message;
    };

    return {
        Initialize: initialize,
        ShowFriends: showFriendsMarkers,
        HideFriends: hideFriendsMarkers,
        UserInfo: userInfo,
        LoadFriendsLiks: loadFriendsLiks,
        InitEventDetails: initEventDetails,
        CreateEventShareMessage: createEventShareMessage
    };
}(jQuery);