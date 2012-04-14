using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace YouMap.ActionFilters
{
    public class RedirectMobileDevicesAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (((filterContext.RouteData.Values["area"] as string) ?? string.Empty).ToLower() == "mobile")
            {
                base.OnActionExecuting(filterContext);
            }
            if (filterContext.HttpContext.Request.Browser.IsMobileDevice)
            {
                var isFullView = filterContext.HttpContext.Session["FullView"];
                if (isFullView == null || !(bool) isFullView)
                {
                    filterContext.Result = new RedirectResult("/Mobile");
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class RedirectMobileDevicesToMobileAreaAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            // Only redirect on the first request in a session
            //if (!httpContext.Session.IsNewSession)
            //    return true;

            // Don't redirect non-mobile browsers
            if (!httpContext.Request.Browser.IsMobileDevice)
                return true;

            // Don't redirect requests for the Mobile area
            if (Regex.IsMatch(httpContext.Request.Url.PathAndQuery, "/Mobile($|/)"))
            {
                return true;
            }
            var isFullView = httpContext.Session["FullView"];
            if (isFullView != null && (bool)isFullView)
            {
                return true;
            }
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var redirectionRouteValues = GetRedirectionRouteValues(filterContext.RequestContext);
            filterContext.Result = new RedirectToRouteResult(redirectionRouteValues);
        }

        // Override this method if you want to customize the controller/action/parameters to which
        // mobile users would be redirected. This lets you redirect users to the mobile equivalent
        // of whatever resource they originally requested.
        protected virtual RouteValueDictionary GetRedirectionRouteValues(RequestContext requestContext)
        {
            return new RouteValueDictionary(new {area = "Mobile", controller = "Home", action = "Main"});
        }
    }
}