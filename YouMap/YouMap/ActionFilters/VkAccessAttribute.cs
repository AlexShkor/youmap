using System.Web.Mvc;
using YouMap.Domain.Enums;

namespace YouMap.ActionFilters
{
    public class VkAccessAttribute: ActionFilterAttribute
    {
        private readonly ISessionContext _sessionContext = new SessionContext();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (_sessionContext.User != null && _sessionContext.User.HasPermissions(UserPermissionEnum.VkUser))
            {
                base.OnActionExecuting(filterContext);
                return;
            }
            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new PartialViewResult()
                                           {
                                               ViewName = "VkError"
                                           };
            }
            else
            {
                filterContext.Result = new ViewResult()
                                           {
                                               ViewName = "VkError"
                                           };
            }
        }
    }
}