using System;
using System.Web.Mvc;
using Paralect.Domain;
using Prelude.Extensions;
using StructureMap.Attributes;
using YouMap.Domain.Auth;
using YouMap.Domain.Data;
using YouMap.Domain.Enums;
using YouMap.Framework;
using YouMap.Framework.Exceptions;
using YouMap.Framework.Mvc.Ajax;

namespace YouMap.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly Location DefaultLocation = Location.Parse("53.9022474151841", "27.561811187172"); //Minsk central square 

        #region Properties

        private AjaxResponse _response;
        protected readonly TimeSpan  LocationUpdateDelay = TimeSpan.FromMinutes(30);

        public virtual AjaxResponse AjaxResponse
        {
            get { return _response ?? (_response = new AjaxResponse(this)); }
        }

        #region Permissions

        protected bool IsAdvertiser
        {
            get { return User.HasPermissions(UserPermissionEnum.Advertiser); }
        }

        protected bool IsAdmin
        {
            get { return User.HasPermissions(UserPermissionEnum.Admin); }
        }

        protected bool IsVkUser
        {
            get { return User.HasPermissions(UserPermissionEnum.VkUser); }
        }

        #endregion

        
        public void UpdateModelIfPost<TModel>(TModel model) where TModel:class 
        {
            if (IsPost)
            {
                TryUpdateModel(model);
            }
        }

        protected bool IsPost { get { return Request.HttpMethod.Equals("post", StringComparison.InvariantCultureIgnoreCase); } }

        protected ICommandService CommandService { get; set; }

        [SetterProperty]
        protected ISessionContext SessionContext { get; set; }

        protected new IUserIdentity User {get { return SessionContext.User; }}

        #endregion

        protected BaseController(ICommandService commandService)
        {
            SessionContext = SessionContext ?? new SessionContext();
            CommandService = commandService;
        }

        public virtual void Send(params ICommand[] commands)
        {
            if (SessionContext.IsUserAuthorized())
            {
                foreach (var command in commands)
                {
                    command.Metadata.UserId = SessionContext.User.Id;
                }
            }

            CommandService.Send(commands);
        }

        //protected override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    var result = filterContext.Result as ViewResult;
        //    if (result !=  null)
        //    {
        //        if (String.IsNullOrEmpty(result.ViewName))
        //        {
        //            result.ViewName = filterContext.ActionDescriptor.ActionName;
        //        }
        //        result.ViewName = Request.Browser.IsMobileDevice ? result.ViewName + ".Mobile" : result.ViewName;
        //    }
        //    base.OnActionExecuted(filterContext);
        //}

        protected ActionResult RespondTo(object model = null, string view = null)
        {
            return RespondTo(request =>
            {
                request.Html = () => model != null ? View(view,model) : View(view);
                request.Ajax = () => model != null ? PartialView(view,model) : PartialView(view);
                request.Json = () => Result();
            });
        }


        public void SendAsync(params ICommand[] commands)
        {
            if (SessionContext.IsUserAuthorized())
            {
                foreach (var command in commands)
                {
                    command.Metadata.UserId = SessionContext.User.Id;
                }
            }

            CommandService.SendAsync(commands);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.CurrentLocation = SessionContext.Location ?? DefaultLocation;
            if ((DateTime.Now - SessionContext.LastLocationUpdate) > LocationUpdateDelay)
            {
                ViewBag.NeedToUpdateLocation = true;
            }
            base.OnActionExecuting(filterContext);
        }


        #region Responses

        public virtual ActionResult Result()
        {
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        AjaxResponse.ValidationContext.AddError(error.ErrorMessage, key);
                    }
                }
            }

            return Json(AjaxResponse.ToJsonObject(), JsonRequestBehavior.AllowGet);
        }

        protected ActionResult RespondTo(Action<RequestFormatResponder> block)
        {
            var responder = new RequestFormatResponder();

            if (block != null)
                block(responder);

            var result = responder.Respond(ControllerContext);
            if (result != null)
                return result;

            throw new HttpNotFoundException("Unable to respond to requested format.");
        }

        protected new JsonResult Json(object data)
        {
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        
        #endregion

    }
}
