using System;
using System.Web.Mvc;

namespace YouMap.ActionFilters
{
    public class MobileAttribute: ActionFilterAttribute
    {
        private const bool _alwaysMobile = false;

        public MobileAttribute()
        {
        }

        //public override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    var result = filterContext.Result as ViewResult;
        //    if (result != null)
        //    {
        //        if (String.IsNullOrEmpty(result.ViewName))
        //        {
        //            result.ViewName = filterContext.ActionDescriptor.ActionName;
        //        }
        //        result.ViewName =filterContext.RequestContext.HttpContext.Request.Browser.IsMobileDevice ? result.ViewName + ".Mobile" : result.ViewName;
        //    }
        //    base.OnActionExecuted(filterContext);
        //}

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.Browser.IsMobileDevice && !_alwaysMobile)
            {
                return;
            }
            if (filterContext.HttpContext.Request.QueryString["mobile"] == "false")
            {
                return;
            }
            var viewResult = filterContext.Result as ViewResult;
            var partialResult = filterContext.Result as PartialViewResult;
            if (partialResult != null)
            {
                if (String.IsNullOrEmpty(partialResult.ViewName))
                {
                    partialResult.ViewName = filterContext.RouteData.Values["action"].ToString();
                }
                partialResult.ViewName += ".Mobile";
                return;
            }
            if (viewResult != null)
            {
                if (String.IsNullOrEmpty(viewResult.ViewName))
                {
                    viewResult.ViewName = filterContext.RouteData.Values["action"].ToString();
                }
                viewResult.ViewName += ".Mobile";
            }
        }


    }
}