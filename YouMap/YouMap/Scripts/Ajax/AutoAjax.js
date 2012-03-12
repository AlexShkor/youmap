
$(document).ready(function () {
    $(document).on("click", ".ajax-link", function (event) {
        var url = $(this).attr("href");
        Request.get(url).addSuccess("resize", YouMap.Map.SetMapHeight).send();
        return false;
    });

    $(document).on("click", ".colorbox", function (event) {
        var url = $(this).attr("href");
        $.colorbox({
            href: url,
            onComplete: function() {
                $.colorbox.resize();
            },
            overlayClose: true,
            scrolling: false,
            close: "<a>закрыть</a>"
        });
        return false;
    });

    $(document).on("click", ".ajax-submit", function (event) {
        var form = $(this).parents("form");
        event.preventDefault();
        if (!validate(form[0])) {
            return;
        }
        Request.named(form.attr("action")).setForm(form).addSuccess("resize", YouMap.Map.SetMapHeight).send();
        return false;
    });

    $("#cboxContent input[type='submit']").live("click", function () {
        setTimeout(function() {
            $.colorbox.resize();
        }, 1);
    });
    
    var data_validation = "unobtrusiveValidation";
    function validate(form) {
        var validationInfo = $(form).data(data_validation);
        return !validationInfo || !validationInfo.validate || validationInfo.validate();
    }

    
});