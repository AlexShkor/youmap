using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Prelude.Extensions.FlashMessage;
using YouMap.Framework.Enums;
using YouMap.Framework.Utils.Extensions;

namespace YouMap.Framework.Mvc.Helpers
{
    public static class HtmlHelpers
    {
        private const string pubDir = "/TenantsContent/Default/Content";
        private const string viewDir = "/TenantsContent/Default/Views";
        private const string chartDir = "charts/FusionCharts";
        private const string cssDir = "css";
        private const string imageDir = "images";
        private const string scriptDir = "scripts";
        private const string flashDir = "flash";
        private const string videoDir = "videos";


        public static string FormatNullableDate(this HtmlHelper helper, DateTime? time)
        {
            var dateTime = "N/A";
            if (time != null && ((DateTime)time).Year != 1)
                dateTime = ((DateTime)time).ToShortDateString();
            return dateTime;
        }

        public static string DatePickerEnable(this HtmlHelper helper)
        {
            var sb = new StringBuilder();
            sb.AppendLine(
                "<script type='text/javascript'>$(document).ready(function() {$('.date-selector').datepicker();});</script>\n");
            return sb.ToString();
        }

        public static MvcHtmlString Chop(this HtmlHelper helper, string content, int length)
        {
            return MvcHtmlString.Create(String.Format("{0} ...", content.Substring(0, length)));

        }

        public static MvcHtmlString DisplayIf(this HtmlHelper helper, bool display)
        {
            return MvcHtmlString.Create(display ? "style=display:none" : String.Empty);

        }

        public static MvcHtmlString Script(this HtmlHelper helper, string fileName)
        {
            if (!fileName.EndsWith(".js"))
                fileName += ".js";

            var filePath = GetFilePath(helper.ViewContext.HttpContext, scriptDir, fileName);
            filePath = ScriptVersion.Instance.AppendScriptVersion(filePath);

            string jsPath = String.Format("<script type='text/javascript' src='{0}' ></script>\n", filePath);

            return MvcHtmlString.Create(jsPath);
        }

        public static IHtmlString FlashMessage(this HtmlHelper instance, string tagName = "p", bool encoded = true)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Func<string, XNode> content = message => encoded ? new XText(message) : XElement.Parse(message) as XNode;

            var messages = new FlashStorage(instance.ViewContext.TempData).Messages.ToList();

            var elements = messages.Select(pair => new XElement(tagName, new XAttribute("class", "flash" + " " + pair.Key), content(pair.Value)));
            var html = String.Join(System.Environment.NewLine, elements.Select(e => e.ToString()));

            return instance.Raw(html);
        }

        public static MvcHtmlString Flash(this HtmlHelper helper, string fileName)
        {
            var filePath = FlashPath(helper, fileName);

            // Savan ---
            // Modify this to insert the object code for plashing flash files.
            string flashCode = String.Format("<script type='text/javascript' src='{0}' ></script>\n", filePath);

            return MvcHtmlString.Create(flashCode);
        }

        public static string FlashPath(this HtmlHelper helper, string fileName)
        {
            if (!fileName.EndsWith(".swf")) {fileName += ".swf";}

            return GetFilePath(helper.ViewContext.HttpContext, flashDir, fileName);
        }

        public static MvcHtmlString Css(this HtmlHelper helper, string fileName, string media = "screen", bool cascade = true)
        {
            if (!fileName.EndsWith(".css"))
                fileName += ".css";

            // Searches in the default project for the css with the filename
            var defaultFilePath = String.Format("{0}/{1}/{2}", pubDir, cssDir, fileName);
            var defaultCssPath =
                String.Format("<link rel='stylesheet' type='text/css' href='{0}'  media='" + media + "'/>\n", defaultFilePath);

            if (!FileExists(defaultFilePath))
                defaultCssPath = String.Empty;

            // Searches in the tenant folder for the css with the filename
            var filePath = GetFilePath(helper.ViewContext.HttpContext, cssDir, fileName);
            filePath = ScriptVersion.Instance.AppendCssVersion(filePath);
            var tenantCssPath =
                String.Format("<link rel='stylesheet' type='text/css' href='{0}'  media='" + media + "'/>\n", filePath);

            if (cascade)
                //If file paths are the same, return only one link tag, else return both link tags for cascading of default down to the tenant
                return String.Equals(defaultCssPath, tenantCssPath) ? MvcHtmlString.Create(defaultCssPath) : MvcHtmlString.Create(String.Concat(defaultCssPath, tenantCssPath));
            else
                return String.Equals(defaultCssPath, tenantCssPath) ? MvcHtmlString.Create(defaultCssPath) : MvcHtmlString.Create(String.Concat(tenantCssPath));

        }

        public static MvcHtmlString Image(this HtmlHelper helper, string fileName, string attributes = "")
        {
            var filePath = GetFilePath(helper.ViewContext.HttpContext, imageDir, fileName);
            return MvcHtmlString.Create(String.Format("<img src='{0}' {1} />", filePath, attributes));
        }

        public static MvcHtmlString ImagePath(this HtmlHelper helper, string fileName)
        {
            var filePath = GetFilePath(helper.ViewContext.HttpContext, imageDir, fileName);
            return MvcHtmlString.Create(String.Format("{0}", filePath));
        }

        public static MvcHtmlString FavIcon(this HtmlHelper helper, string fileName = "favicon.ico")
        {
            var filePath = GetFilePath(helper.ViewContext.HttpContext, imageDir, fileName);

            var jsPath =
                String.Format("<link rel='shortcut icon'  type='image/x-icon' href='{0}'/>\n", filePath);

            return MvcHtmlString.Create(jsPath);
        }



        public static MvcHtmlString StatusMessage(this HtmlHelper helper, string message)
        {
            if (String.IsNullOrEmpty(message))
                return MvcHtmlString.Empty;

            var @class = String.Empty;

            var messageTypeToCss = new Dictionary<StatusMessageType, string>
                                       {
                                           {StatusMessageType.Success, "validation-summary-success"},
                                           {StatusMessageType.Warning, "validation-summary-success"},
                                           {StatusMessageType.Error, "validation-summary-errors"},
                                       };

            foreach (var map in messageTypeToCss)
            {
                var tag = String.Format(">>{0}:", map.Key);
                if (message.Contains(tag))
                {
                    @class = map.Value;
                    message = message.Replace(tag, String.Empty);
                }
            }

            var html = new StringBuilder();
            html.AppendFormat("<div class='{0}'><ul><li>{1}</li></ul></div>", @class, message);

            return MvcHtmlString.Create(html.ToString());
        }


        //public static string Layout(this HtmlHelper helper, string fileName, string areaName = null)
        //{
        //    if (fileName.EndsWith(".cshtml"))
        //        fileName += fileName.Replace(".cshtml", "");

        //    if (areaName != null)
        //        helper.ViewContext.Controller.ControllerContext.RouteData.Values["area"] = areaName;

        //    var viewEngine = (ThemedViewEngine)ViewEngines.Engines.FirstOrDefault();

        //    Ensure.NotNull(viewEngine);

        //    return viewEngine.FindViewPath(helper.ViewContext.Controller.ControllerContext, fileName);
        //}

        public static MvcHtmlString Spinner(this HtmlHelper helper, string imageName, string attributes = "")
        {
            var filePath = GetFilePath(helper.ViewContext.HttpContext, imageDir, imageName);

            string tag = String.Format("<div class='spinner' style='display: none;'><img src='{0}' {1} /></div>", filePath, attributes);

            return MvcHtmlString.Create(tag);
        }


        #region Chart Helpers

        //public static MvcHtmlString CreateFusionChartJavascript(this HtmlHelper helper, FusionChart fusionChart)
        //{
        //    if (fusionChart == null)
        //        return MvcHtmlString.Create(String.Empty);

        //    StringBuilder fusionChartJavascript = new StringBuilder();

        //    fusionChartJavascript.AppendFormat("<div id='{0}Container' align='center'></div>" + Environment.NewLine, fusionChart.ChartDivId);
        //    fusionChartJavascript.AppendLine("<script type=\"text/javascript\">" + Environment.NewLine + "FusionCharts._fallbackJSChartWhenNoFlash();");
        //    fusionChartJavascript.AppendFormat("var chart_{0} = new FusionCharts(\"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\" , \"0\");" + Environment.NewLine,
        //                         fusionChart.ChartDivId,
        //                         GetFilePath(helper.ViewContext.HttpContext, "charts/" + fusionChart.Type, fusionChart.ChartName + ".swf"),
        //                         fusionChart.ChartDivId,
        //                         fusionChart.Width,
        //                         fusionChart.Height,
        //                         fusionChart.Debugging);

        //    fusionChartJavascript.AppendFormat("chart_{0}.setDataXML(\"{1}\");" + Environment.NewLine, fusionChart.ChartDivId, fusionChart.GetXml());
        //    fusionChartJavascript.AppendFormat("chart_{0}.render(\"{1}Container\");" + Environment.NewLine, fusionChart.ChartDivId, fusionChart.ChartDivId);
        //    fusionChartJavascript.Append("</script>" + Environment.NewLine);

        //    return MvcHtmlString.Create(fusionChartJavascript.ToString());
        //}

        #endregion

        //public static MvcHtmlString Money(this HtmlHelper helper, Int64 amount)
        //{
        //    return MvcHtmlString.Create(amount.ToString("C2").Replace("$", String.Empty));
        //}

        public static string GetFilePath(HttpContextBase context, string contentTypeDir, string fileName)
        {
            string filePath = String.Empty;

            if (context == null)
                throw new InvalidOperationException("Http Context cannot be null.");

            var baseUrl = context.Request.Url.BaseUrl().TrimEnd('/');


            if (context.Request.Cookies["EmulationUrl"] != null)
                baseUrl = context.Request.Cookies["EmulationUrl"].Value;


            var cacheKey = String.Format(CultureInfo.InvariantCulture, "content_for_{0}", baseUrl);

            var virtualPath = String.Concat((string)context.Cache[cacheKey],
                                            contentTypeDir,
                                            "/",
                                            fileName);


            if (FileExists(virtualPath))
                filePath = virtualPath.TrimStart('~');
            else if (FileExists(String.Concat(pubDir, "/", contentTypeDir, "/", fileName)))
                filePath = String.Format("{0}/{1}/{2}", pubDir, contentTypeDir, fileName);

            return filePath;
        }

        private static bool FileExists(string virtualPath)
        {
            var context = HttpContext.Current;
            if (context == null)
                throw new InvalidOperationException("Http Context cannot be null.");

            var serverPath = context.Server.MapPath(virtualPath);

            return File.Exists(serverPath);
        }
    }
}
