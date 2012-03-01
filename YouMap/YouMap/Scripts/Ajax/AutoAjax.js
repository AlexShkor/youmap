
$(document).ready(function () {
    $(document).on("click", ".ajax-link", function () {
        var url = $(this).attr("href");
        Request.get(url).send();
        return false;
    });

    $(document).on("click", ".colorbox", function () {
        var url = $(this).attr("href");
        $.colorbox({ href: url });
        return false;
    });

    $(document).on("click", ".ajax-submit", function () {
        var form = $(this).parents("form");
        Request.named(form.attr("action")).setForm(form).send();
        return false;
    });
});