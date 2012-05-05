using System;
using System.Web.Mvc;
using System.Web.Security;
using YouMap.ActionFilters;
using YouMap.Domain.Auth;
using YouMap.Domain.Enums;
using YouMap.Framework;
using YouMap.Models;

namespace YouMap.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;

        private VkAuthenticationService VkAuth
        {
            get { return _authenticationService.VkAuth; }
        }

        public AccountController(
            ICommandService commandService, 
            IAuthenticationService authenticationService) : base(commandService)
        {
            _authenticationService = authenticationService;
        }

        public ActionResult LogOn()
        {
            return RespondTo(new LogOnModel());
        }

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
      
        [HttpGet]
        public ActionResult SetupAdmin()
        {
            if (_authenticationService.HasAdmin)
            {
                return HttpNotFound();
            }
            return View("Register", new RegisterModel());
        }

        [HttpPost]
        public ActionResult SetupAdmin(RegisterModel model)
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
            return RespondTo(model, "Register");
        }

        [HttpGet]
        public ActionResult Register()
        {
            var model = new RegisterModel();
            //TODO: Check, does it work?
            UpdateModelIfPost(model);
            return RespondTo(model);
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return Register();
            }
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

        public ActionResult Vk()
        {
            return View("VkError");
        }

        [HttpGet]
        public ActionResult LoginVk()
        {
            try
            {
                var user = VkAuth.LoginFromCookie(Request.Cookies);
                FormsAuthentication.SetAuthCookie(user.Name, true);
                SessionContext.SetUser(user);
                AjaxResponse.AddJsonItem("Success",true);
                RenderTopBar();
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
                    RenderTopBar();
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError("Error",exception.Message);
                }            
            }
            return Result();
        }

        [HttpPost]
        //TODO: remove it
            //UNSAFE
        public ActionResult LoginVkStatus(string uid)
        {
            try
            {
                _authenticationService.LoginWithUid(uid);
                RenderTopBar();
            }
            catch (Exception exception)
            {
                ModelState.AddModelError("Error", exception.Message);
            }
            return Result();
        }

        private void RenderTopBar()
        {
            AjaxResponse.Render("#topBar", "TopBar", new{});
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
                model.DisplayName = user.Name ?? user.Email;
                model.DisplayAdmin = user.HasPermissions(UserPermissionEnum.Admin);
            }
            return model;
        }

        public ActionResult Profile()
        {
            return PartialView();
        }
    }
}
