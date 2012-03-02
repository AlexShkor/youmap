using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using YouMap.Documents.Services;
using YouMap.Models;
using mPower.Framework;
using mPower.Framework.Environment;

namespace YouMap.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;
        VkAuthenticationService VkAuth
        {
            get { return _authenticationService.VkAuth; }
        }
        //
        // GET: /Account/LogOn

        public AccountController(
            ICommandService commandService, 
            IAuthenticationService authenticationService) : base(commandService)
        {
            _authenticationService = authenticationService;
        }

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _authenticationService.LogOn(model.Email,model.Password);
                     if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Map");
                    }
                }
                catch
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            return View(model);
        }

        public ActionResult LogOff()
        {
            _authenticationService.Logout();
            return RedirectToAction("Index", "Map");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _authenticationService.Register(model.Email, model.Password);
                    return RedirectToAction("Index", "Map");
                }
                catch(Exception e)
                {
                    ModelState.AddModelError("Error",e.Message);
                }
            }
            return View(model);
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LoginVk()
        {
            try
            {
                var user = VkAuth.LoginFromCookie(Request.Cookies);
                FormsAuthentication.SetAuthCookie(user.Name, true);
                SessionContext.SetUser(user);
            }
            catch(Exception e)
            {
               ModelState.AddModelError("Error",e.Message);
            }
            return Result();
        }

        [HttpPost]
        public ActionResult LoginVk(VkLoginModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _authenticationService.LogOn(model);
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError("Error",exception.Message);
                }
                AjaxResponse.Render("#logindisplay","_LogOnPartial",new{});
            }
            return Result();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _authenticationService.ChangePassword(model.OldPassword, model.NewPassword);
                    return RedirectToAction("ChangePasswordSuccess");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }
            return View(model);
        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }
    }

    public class VkLoginModel
    {
        public long Expire { get; set; }

        [Required]
        public string Sig { get; set; }

        [Required]
        public string Sid { get; set; }

        [Required]
        public string Mid { get; set; }

        public string Secret { get; set; }


        public String Domain { get; set; }

        [Required]
        public String FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Id { get; set; }

        public String Href { get; set; }

        public String Nickname { get; set; }
    }
}
