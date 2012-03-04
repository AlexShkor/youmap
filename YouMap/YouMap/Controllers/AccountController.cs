using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using YouMap.Documents.Services;
using YouMap.Domain.Auth;
using YouMap.Domain.Enums;
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

        [HttpGet]
        public ActionResult SetupAdmin()
        {
            if (_authenticationService.HasAdmin)
            {
                return HttpNotFound();
            }
            return View("LogOn", new LogOnModel());
        }

        [HttpPost]
        public ActionResult SetupAdmin(LogOnModel model)
        {
            if (_authenticationService.HasAdmin)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _authenticationService.CreateAdmin(model.Email, model.Password);
                    return RedirectToAction("Index", "Map");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("Error", e.Message);
                }
            }
            return View("LogOn", model);
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
                    AjaxResponse.Render("#logindisplay", "LoginState", Map(User));
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError("Error",exception.Message);
                }            
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
                model.DisplayName = user.Name;
                model.DisplayAdmin = user.HasPermissions(UserPermissionEnum.Admin);
            }
            return model;
        }
    }
}
