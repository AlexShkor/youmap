

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
                YouMap.Map.SetMapCenter(ui.item.Latitude, ui.item.Longitude);
                return false;
            }
        })
		.data("autocomplete")._renderItem = function (ul, item) {

		    return $("<li></li>")
				.data("item.autocomplete", item)
	            .append("<img src='" + item.Icon + "'/>")
				.append("<a>" + "<strong>" +item.Title + " </strong><span>" + item.Address + "</span></a>")		
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

    return {
        Initialize: initialize
    };
}(jQuery)