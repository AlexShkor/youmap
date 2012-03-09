using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Paralect.Domain;
using Prelude.Extensions;
using StructureMap.Attributes;
using YouMap.Domain.Auth;
using YouMap.Domain.Enums;
using mPower.Framework;
using mPower.Framework.Exceptions;
using mPower.Framework.Mvc.Ajax;

namespace YouMap.Controllers
{
    public abstract class BaseController : Controller
    {
        #region Properties

        private AjaxResponse _response = null;

        public virtual AjaxResponse AjaxResponse
        {
            get
            {
                if (_response == null)
                    _response = new AjaxResponse(this);
                return _response;
            }
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

        protected ActionResult RespondTo(object model = null, string view = null)
        {
            return RespondTo(request =>
            {
                request.Html = () => model != null ? View(view,model) : View(view);
                request.Ajax = () => model != null ? PartialView(view,model) : PartialView(view);
                request.Json = () => Result();
            });
        }


        //public void SendAsync(params ICommand[] commands)
        //{
        //    if (SessionContext.IsUserAuthorized())
            //{
            //    foreach (var command in commands)
            //    {
            //        command.Metadata.UserId = SessionContext.UserId;
            //    }
            //}

        //    CommandService.SendAsync(commands);
        //}


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
