
$(document).ready(function () {
    $(document).on("click", ".ajax-link", function (event) {
        var url = $(this).attr("href");
        Request.get(url).addSuccess("resize", YouMap.Map.SetMapHeight).send();
        return false;
    });

    $(document).on("click", ".colorbox", function (event) {
        var url = $(this).attr("href");
        $.colorbox({ href: url });
        return false;
    });

    $(document).on("click", ".ajax-submit", function (event) {
        var form = $(this).parents("form");
        Request.named(form.attr("action")).setForm(form).addSuccess("resize", YouMap.Map.SetMapHeight).send();
        return false;
    });
    
});