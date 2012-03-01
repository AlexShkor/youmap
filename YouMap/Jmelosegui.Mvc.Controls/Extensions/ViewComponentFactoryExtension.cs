using System.Diagnostics;
using Telerik.Web.Mvc.Infrastructure;
using Telerik.Web.Mvc.UI;

namespace Jmelosegui.Mvc.Controls
{
    public static class ViewComponentFactoryExtension
    {

        [DebuggerStepThrough]
        public static GoogleMapBuilder GoogleMap(this ViewComponentFactory source)
        {
            Guard.IsNotNull(source, "source");
            return ComponentBuilderBase<GoogleMap, GoogleMapBuilder>.Create(source.Register(
                () => new GoogleMap(source.HtmlHelper.ViewContext, source.ClientSideObjectWriterFactory)));
        }
    }
}
