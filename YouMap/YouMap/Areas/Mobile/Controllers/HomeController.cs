using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouMap.Controllers;
using YouMap.Documents.Services;
using YouMap.Framework;

namespace YouMap.Areas.Mobile.Controllers
{
    public class HomeController : MapController
    {
        private UserDocumentService _userDocumentService;
        private AuthenticationService _authenticationService;
        public HomeController(ICommandService commandService, PlaceDocumentService placeDocumentService, ImageService imageService,UserDocumentService userDocumentService, AuthenticationService authenticationService) : base(commandService, placeDocumentService, imageService, userDocumentService)
        {
            _userDocumentService = userDocumentService;
            _authenticationService = authenticationService;
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Main()
        {
            ViewBag.VkAuthUrl = string.Format(
                "http://oauth.vk.com/authorize?client_id={0}scope={1}&redirect_uri={2}&display=touch&response_type=token",
                Framework.Settings.Current.VkAppId, 1027,
                "http://" + Request.Url.Authority + Url.Action("VkAuthCallback"));
            return View("Index");
        }

        public ActionResult Settings()
        {
            throw new NotImplementedException();
        }

        public ActionResult VkAuthCallback(string access_token, string expires_in, string user_id)
        {
            var user = _userDocumentService.GetByFilter(new UserFilter() {VkId = user_id}).FirstOrDefault();
            if (user !=null)
            {
                _authenticationService.SetAuthCookie(user, true);
            }
            else
            {
                ViewBag.AutoLogin = true;   
            }            
            return View("Index");
        }
    }
}
