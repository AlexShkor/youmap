YouMap.Auth = function ($) {

    var loginned = false;
    var initialize = function (isUserLoginned) {
        loginned = isUserLoginned;
        if (!loginned) {
            $("#vkLogin").live("click", function() {
                VK.Auth.login(vkLoginCallback);
            });
            Request.get("/Account/LoginVk").addSuccess("loginVK", function(data) {
                if (false) {
                    VK.Auth.getLoginStatus(vkLoginCallback);
                }
            }).send();
        }
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