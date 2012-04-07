YouMap.Auth = function ($) {

    var permissions = 2 + 1024 + 1;
    var loginned = false;
    var initialize = function (isUserLoginned) {
        loginned = isUserLoginned;
            $("#vkLogin").live("click", function() {
                VK.Auth.login(vkLoginCallback, permissions);
                return false;
            });
            //setTimeout(function() {
            //    VK.Auth.getLoginStatus(getStatusCallback);
        //}, 1);
           // $("#vkMobileLogin").attr("href", "http://oauth.vk.com/authorize?client_id=2831071&response_type=token&scope=1027" + false ? "&redirect_uri=localhost" : "&redirect_uri=localhost");
    };

    var vkLoginCallback = function (response) {
        if (response.session) {
            if ($("#vkErrorPopup").length > 0) {
                $.colorbox.close();
            }
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
            VK.Api.call("friends.get", {fields: "uid, first_name, last_name" }, function (result) {
                if (result && result.response) {
                    Request.post("/Users/UpdateFriends").addParams({ json: JSON.stringify(result.response) }).send();
                }
            });
            if (response.settings) {
                /* Выбранные настройки доступа пользователя, если они были запрошены */
            }
        } else {
            /* Пользователь нажал кнопку Отмена в окне авторизации */
        }
    };

    var autoLogin = function() {
        VK.Auth.login(vkLoginCallback, permissions);
    };

    return {
        Initialize: initialize,
        AutoLogin : autoLogin
    };
} (jQuery);