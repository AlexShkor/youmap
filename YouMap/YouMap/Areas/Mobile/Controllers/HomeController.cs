using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using YouMap.Controllers;
using YouMap.Documents.Services;
using YouMap.Domain.Auth;
using YouMap.Domain.Enums;
using YouMap.Framework;
using YouMap.Framework.Utils.Extensions;
using YouMap.Models;

namespace YouMap.Areas.Mobile.Controllers
{
    public class HomeController : MapController
    {
        private readonly UserDocumentService _userDocumentService;
        private readonly AuthenticationService _authenticationService;
        public HomeController(ICommandService commandService, PlaceDocumentService placeDocumentService, ImageService imageService,UserDocumentService userDocumentService, AuthenticationService authenticationService) : base(commandService, placeDocumentService, imageService, userDocumentService)
        {
            _userDocumentService = userDocumentService;
            _authenticationService = authenticationService;
        }


        public ActionResult Main()
        {
            return View();
        }

        public ActionResult Settings()
        {
            return View();
        }

        [HttpGet]
        public ActionResult VkAuthCallback(string code)
        {
            var wc = new WebClient();
            var result = String.Empty;
            try
            {
                result =
                    wc.DownloadString(
                        String.Format("https://oauth.vk.com/access_token?client_id={0}&client_secret={1}&code={2}",
                                      Framework.Settings.Current.VkAppId,
                                      Framework.Settings.Current.VkAppSecret, 
                                      code));
            }
            catch (Exception)
            {
            }
            var js = new JavaScriptSerializer();
            var response = js.Deserialize<dynamic>(result) as Dictionary<string, object>;
            if (response == null || !response.Keys.Contains("user_id"))
            {
                return View("VkError");
            }
            var userId = response["user_id"].ToString();
            var user = _userDocumentService.GetByFilter(new UserFilter() {VkId = userId}).FirstOrDefault();
            if (user == null)
            {
                return View("Login");
            }
            _authenticationService.SetAuthCookie(user, true);
            return View("Main");
        }

        public ActionResult LoginState()
        {
            var model = Map(User);
            return PartialView(model);
        }

        private UserViewModel Map(IUserIdentity user)
        {
            var model = new UserViewModel();
            model.IsAuthenticated = SessionContext.IsUserAuthorized();
            if (model.IsAuthenticated)
            {
                model.DisplayName = user.Name ?? user.Email;
                model.DisplayAdmin = user.HasPermissions(UserPermissionEnum.Admin);
            }
            model.VkLoginUrl = string.Format(
                "http://oauth.vk.com/authorize?client_id={0}&scope={1}&redirect_uri={2}?&display=touch&response_type=code",
                Framework.Settings.Current.VkAppId, 1027,
                "http://" + Request.Url.Authority + Url.Action("VkAuthCallback"));
            return model;
        }

        protected override PlaceModel Map(Documents.Documents.PlaceDocument doc)
        {
            var result =  base.Map(doc);
            result.InfoWindowUrl = Url.Action("Details", "Places", new {id = doc.Id});
            result.OpenInNewWindow = true; 
            return result;
        }
    }
}
