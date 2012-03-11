using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace YouMap.Framework.Utils.Extensions
{
    public static class ControllerExtentions
    {
        public static void ClearErrorFor<T, TOut>(this Controller controller,T model, Expression<Func<T, TOut>> expression)
        {
            controller.ClearErrorFor(expression);
        }

        public static void ClearError(this Controller controller, string name)
        {
            if (controller.ModelState.ContainsKey(name))
            {
                controller.ModelState[name].Errors.Clear();
            }
        }

        public static void ClearErrorFor<T, TOut>(this Controller controller, Expression<Func<T, TOut>> expression)
        {
            var name = ExpressionHelper.GetExpressionText(expression);
            controller.ClearError(name);
        }

        public static string RenderViewToString(this Controller controller, string viewName, object model = null)
        {
            return MvcUtils.RenderPartialToStringRazor(controller.ControllerContext, viewName, model, controller.ViewData, controller.TempData);
        }
    }
}
