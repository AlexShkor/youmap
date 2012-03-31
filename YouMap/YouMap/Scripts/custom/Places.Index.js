

$(document).ready(function() {

    $(".tags a").click(function () {     
        switchAltText(this);
        if (isAltState(this)) {
            var initialTags = getItems(this);
            $(this).parent().find("ul").tagit({
                initialTags: initialTags
            });
        } else {
            var items = getItems(this);
            $(this).parent().find("ul").remove();
            var ul = $("<ul/>");
            for (var i = 0; i < items.length; i++) {
                ul.append($("<li/>").append(items[i]));
            }
            Request.post("Places/EditTags").addParams({
                tags: items.join(","),
                placeId: $(this).data("placeid")
            }).send();
            $(this).before(ul);
        }
    });

    var getItems = function (elem) {
        var initialTags = new Array();
        var tagit = $(elem).parent().find("ul.tagit");
        if (tagit.length > 0) {
            initialTags = tagit.tagit("tags");
        } else {
            $(elem).parent().find("ul li").each(function() {
                initialTags.push($(this).html());
            });
        }
        return initialTags;
    };

    var switchAltText = function(el) {
        var text = $(el).html();
        if ($(el).data("isaltstate")) {
            $(el).data("isaltstate", false);
        } else {
            $(el).data("isaltstate", true);
        }
        $(el).html($(el).data("alttext"));
        $(el).data("alttext", text);
    };

    var isAltState = function(el) {
        if ($(el).data("isaltstate")) {
            return true;
        } else {
            return false;
        }
    };
    
});