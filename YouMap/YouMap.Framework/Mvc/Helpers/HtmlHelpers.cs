using System;
using System.Web.Mvc;

namespace YouMap.Framework.Mvc.Helpers
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString DisplayIf(this HtmlHelper helper, bool display)
        {
            return MvcHtmlString.Create(!display ? "style=display:none" : String.Empty);
        }

        public static MvcHtmlString ClassIf(this HtmlHelper helper, string @class, bool display)
        {
            return MvcHtmlString.Create(display ? @class : String.Empty);
        }

        public static MvcHtmlString CheckedIf(this HtmlHelper helper, bool value)
        {
            return MvcHtmlString.Create(value ? "checked" : String.Empty);

        }
    }
}