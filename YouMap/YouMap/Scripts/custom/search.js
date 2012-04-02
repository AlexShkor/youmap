

YouMap.Search = function($) {

    var SearchUrl = null;
    var initialize = function (searchUrl) {
        SearchUrl = searchUrl;

        $("#searchField").focusin(function() {
            $("#searchSubmit").css("opacity", "1");
        });
        $("#searchField").focusout(function() {
            $("#searchSubmit").css("opacity", "0.1");
        });
        $("#searchField").autocomplete({
            minLength: 2,
            source: function (request, response) {
                Request.get(searchUrl).addParams({ term: request.term }).addSuccess("search", function (r) {
                    var data = r.jsonItems.places;
                    for (var i = 0; i < data.length; i++) {
                        var item = data[i];
                        item.Distance = getDistance(item.X, item.Y)
                    }
                    data.sort(function(a, b) {
                        return a.Distance - b.Distance;
                    });
                    response(data);
                }).send();
            },
            deferRequestBy: 300,
            focus: function (event, ui) {
                $("#searchField").val(ui.item.Title);
                return false;
            },
            select: function (event, ui) {
                $("#searchField").val(ui.item.Title);
                $("#searchField-id").val(ui.item.Id);
                $("#searchField-description").html(ui.item.Description);
                $("#searchField-icon").attr("src", ui.item.Icon);
                //YouMap.Map.SetMapCenter(ui.item.X, ui.item.Y);
                //YouMap.Map.GetPlaceById(ui.item.Id);
                YouMap.Map.NavigateToPlaceById(ui.item.Id);
                return false;
            }
        })
		.data("autocomplete")._renderItem = function (ul, item) {

		    var img = $("<img/>");
		    img.attr("src", item.Icon);
		    var anchor = $("<a/>");
		    anchor.append(img);
		    anchor.append("<strong>" + item.Title + " </strong><span> - " + item.Address + "</span><span style='float:right'>" + item.Distance +" км</span>");
		    return $("<li/>")
				.data("item.autocomplete", item)
	            .append(anchor)
				.appendTo(ul);
		};
        $(".search-google-btn").click(function() {
            YouMap.Map.SearchGoogle($("#searchField").val());
            return false;
        });
        $("#searchField").keydown(function(e) {
            if (e.which == 13) {
                YouMap.Map.SearchGoogle($("#searchField").val());
                return false;
            }
        });
    };

    Number.prototype.toRad = function() {
        return this / 90;
    };

    var getDistance = function(lat1, lon1) {
        var pos = YouMap.Map.GetUserLocation();
        //var lat2 = pos.lat();
        //var lon2 = pos.lng();
        var point = new google.maps.LatLng(lat1, lon1);
        var d = google.maps.geometry.spherical.computeDistanceBetween(pos, point);
        d = d / 1000;
        //var R = 6371; // km
        //var dLat = (lat2 - lat1).toRad();
        //var dLon = (lon2 - lon1).toRad();
        //var lat1 = lat1.toRad();
        //var lat2 = lat2.toRad();

        //var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
        //    Math.sin(dLon / 2) * Math.sin(dLon / 2) * Math.cos(lat1) * Math.cos(lat2);
        //var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        //var d = R * c;
        return d.toFixed(1);
    };
     
    var navigation = function() {
        $("#menu a").click(function() {
            $(this).toggleClass("selected");
            updateMarkersFilter();
            return false;
        });
    };

    var updateMarkersFilter = function() {
        var categories = new Array();
        $("#menu a.selected").each(function() {
            categories.push($(this).attr("categoryid"));
        });
        YouMap.Map.FilterPlaces({
            categories: categories
        });
    };

    return {
        Initialize: initialize,
        Navigation: navigation
    };
}(jQuery)