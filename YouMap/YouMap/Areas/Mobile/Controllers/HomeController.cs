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
                                      2831032, //YpuMap Desktop app id
                                      "RNlgRjT0PjbBMzVtsuyV", //YouMap Desktop app secret
                                      code));
            }
            catch
            {
            }
            var js = new JavaScriptSerializer();
            var response = js.Deserialize<dynamic>(result) as Dictionary<string, object>;
            if (response == null || !response.Keys.Contains("user_id"))
            {
                return View("VkError");
            }
            SessionContext.AccessToken = response["access_token"] as string;
            var userId = response["user_id"].ToString();
            var user = _userDocumentService.GetByFilter(new UserFilter() {VkId = userId}).FirstOrDefault();
            if (user == null)
            {
                try
                {
                    result =
                        wc.DownloadString(
                            String.Format("https://api.vk.com/method/users.get?uids={0}&access_token={1}",
                                          userId,
                                          SessionContext.AccessToken));
                }
                catch
                {
                }
                var users = js.Deserialize<VkArrayResponse<UsersGetDto>>(result).response;
                if (users.Any())
                {
                    var vkUser = users.First();
                     _authenticationService.Register(new VkLoginModel()
                                                    {
                                                        FirstName =vkUser.first_name,
                                                        LastName = vkUser.last_name
                                                    });
                }
               
            }
            else
            {
                _authenticationService.SetAuthCookie(user, true);
            }
            return View("Main");
        }

        protected override void ShareCheckIn(CheckInModel model)
        {
            var wc = new WebClient();
            var result = String.Empty;
            var shareUrl = "http://" + Request.Url.Authority +
                           (model.CheckInUrl ??
                            String.Format("/?latitude={0}?longitude={1}", model.Latitude, model.Longitude));
            try
            {
                result =
                    wc.DownloadString(
                        String.Format("https://api.vk.com/method/wall.post?message={0}&attachments={1}&access_token={2}",
                                      model.Memo,
                                      shareUrl,
                                      SessionContext.AccessToken));
            }
            catch (Exception)
            {
            }
        }

        public ActionResult LoginState()
        {
            var model = Map(User);
            return PartialView(model);
        }
        
        public ActionResult Choise()
        {
            return View();
        }

        public ActionResult SwitchView()
        {
            Session["FullView"] = true;
            return RedirectToAction("Index", "Map", new {area = ""});
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
                2831032, 1027 + 8192,
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

        public ActionResult Logout()
        {
            _authenticationService.Logout();
            return RedirectToAction("Main");
        }
    }

    public class VkArrayResponse<T>
    {
        public List<T> response { get; set; } 
    }
    public class UsersGetDto
    {
        public string uid { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string photo { get; set; }
        public string photo_medium { get; set; }
        public string photo_big { get; set; }
    }
}
