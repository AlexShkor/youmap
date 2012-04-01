using System.Web.Mvc;

namespace YouMap.ActionFilters
{
    public class RedirectMobileDevicesAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
           if (filterContext.HttpContext.Request.Browser.IsMobileDevice)
            {
                filterContext.Result = new RedirectResult("/Mobile");
            }
        }
    }
}