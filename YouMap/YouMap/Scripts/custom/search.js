

YouMap.Search = function($) {

    var SearchUrl = null;
    var initialize = function (searchUrl) {
        SearchUrl = searchUrl;

        $("#searchField").autocomplete({
            minLength: 2,
            source: searchUrl,
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
		    anchor.append("<strong>" + item.Title + " </strong><span>" + item.Address + "</span>");
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