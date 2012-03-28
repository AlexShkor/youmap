YouMap.Google = function($) {

    var createMap = function (options) {
        var innerOptions = {
            zoom: options.Zoom,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            center: new google.maps.LatLng(options.X, options.Y)
        };
        
        var map = new google.maps.Map($("#map")[0], innerOptions);
        return map;
    };
    
    var createImage = function(options) {
        var image = new google.maps.MarkerImage(options.Path,
            new google.maps.Size(options.Size.Width, options.Size.Height),
            new google.maps.Point(options.Point.X, options.Point.Y),
            new google.maps.Point(options.Anchor.X, options.Anchor.Y));
        return image;
    };
    var createMarker = function (map, options) {
        var x = options.X;
        var y = options.Y;
        var markerOptions = {
            position: new google.maps.LatLng(x, y),
            map: map,
            title: options.Title,
            clickable: options.Clickable || true,
            draggable: options.Draggable || false,
            icon: options.Icon ? createImage(options.Icon) : null,
            shadow: options.Shadow ? createImage(options.Shadow) : null
            //animation: google.maps.Animation.DROP
        };
        // create
        var marker = new google.maps.Marker(markerOptions);
        if (options.drag) {
            google.maps.event.addListener(marker, 'drag', function (e) {
                options.drag(e.latLng.lat(), e.latLng.lng());
            });
        }
        var param = options;
        if (options.click) {
            google.maps.event.addListener(marker, 'click', function() {
                options.click(param);
            });
        }
       
        return marker;
    };

    var infowindow = null;
    var openWindow = function(map,marker,content) {
            if (infowindow) {
                infowindow.close();
            }
            infowindow = new google.maps.InfoWindow({ content: content });
            //var myOptions = {
            //    content: content
            //    , disableAutoPan: false
            //    , maxWidth: 0
            //    , pixelOffset: new google.maps.Size(-140, 0)
            //    , zIndex: null
            //    , boxStyle: {
            //        'background-color': "black"
            //      , opacity: 0.75
            //      , width: "280px"
            //    }
            //    , closeBoxMargin: "10px 2px 2px 2px"
            //    , closeBoxURL: "http://www.google.com/intl/en_us/mapfiles/close.gif"
            //    , infoBoxClearance: new google.maps.Size(1, 1)
            //    , isHidden: false
            //    , pane: "floatPane"
            //    , enableEventPropagation: false
            //};
            //infowindow = new InfoBox(myOptions);
            infowindow.open(map, marker);
    };
    var addMarker = function(map, marker) {
        marker.setMap(map);
    };

    var removeMarker = function(marker) {
        marker.setMap(null);
    };
    
    var setPosition = function (marker, x, y) {
        var location = new google.maps.LatLng(x, y);
        marker.setPosition(location);
    };


    return {
        CreateMap: createMap,
        CreateMarker: createMarker,
        AddMarker: addMarker,
        SetPosition: setPosition,
        OpenWindow: openWindow,
        RemoveMarker: removeMarker
    };
}(jQuery);