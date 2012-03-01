YouMap = { };

var getMap = function() {
    return $("#map").data("GoogleMap");
};

var createMarker = function (config, onDrag) {
    var map = getMap();
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

YouMap.Map = function ($) {
    var initialize = function () {

        $(".place-info-window").each(function () {
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
    };

    return { Initialize: initialize };
} (jQuery);

YouMap.ControlPanel = function ($) {

    var initialize = function () {
    };

    return {
        Initialize: initialize
    };
} (jQuery);


YouMap.AddPlace = function ($) {

    var geocoder = null;


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

        if (geocoder == null) {
            geocoder = new google.maps.Geocoder();
        }
    };

    //Установить карту на город 
    var setMapToCity = function () {
        var address = $("#Address").val();
        var map = getMap();

        geocoder.geocode({ 'address': address }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                var location = results[0].geometry.location;
                map.GMap.setCenter(location);
                //установить Zoom таким образом, чтобы город был показан весь
                map.GMap.setZoom(getZoom(results[0].geometry.viewport));
                //и поставить маркет для отметки адреса
                setMarkerPosition(location);
                updateFormFields(results[0].geometry.location);

            } else {
                alert("Пошло что-то не так, потому что: " + status);
            }
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
    return {
        Initialize: initialize
    };
} (jQuery);

YouMap.Auth = function ($) {
    var initialize = function () {
        $("#vkLogin").live("click", function () {
            VK.Auth.login(vkLoginCallback);
        });

        VK.Auth.getLoginStatus(function (response) {
            if (response.session) {
                Request.get("/Account/LoginVk");
            } else {
                /* Неавторизованный в Open API пользователь */
            }
        });
    };

    var vkLoginCallback = function (response) {
        if (response.session) {

            Request.post("/Account/LoginVk").addParams({
                Expire: response.session.expire,
                Sig: response.session.sig,
                Sid: response.session.sid,
                Mid: response.session.mid,
                Secret: response.session.secret,
                Domain: response.session.user.domain,
                FirstName: response.session.user.first_name,
                LastName: response.session.user.last_name,
                Id: response.session.user.id,
                Href: response.session.user.href,
                Nickname: response.session.user.nickname
            }).send();
            if (response.settings) {
                /* Выбранные настройки доступа пользователя, если они были запрошены */
            }
        } else {
            /* Пользователь нажал кнопку Отмена в окне авторизации */
        }
    };
    return {
        Initialize: initialize
    };
} (jQuery);