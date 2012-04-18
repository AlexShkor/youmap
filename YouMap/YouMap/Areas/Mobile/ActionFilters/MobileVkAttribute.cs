
using System.Web.Mvc;
using System.Web.Routing;
using YouMap.ActionFilters;
using YouMap.Framework.Utils.Extensions;

namespace YouMap.Areas.Mobile.ActionFilters
{
    public class MobileVkAttribute: AuthorizeAttribute
    {
        private readonly ISessionContext _sessionContext = new SessionContext();

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            return _sessionContext.AccessToken.HasValue();
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var redirectionRouteValues = GetRedirectionRouteValues(filterContext.RequestContext);
            filterContext.Result = new RedirectToRouteResult(redirectionRouteValues);
        }

        protected virtual RouteValueDictionary GetRedirectionRouteValues(RequestContext requestContext)
        {
            return new RouteValueDictionary(new { area = "Mobile", controller = "Home", action = "Logon" });
        }
    }
}