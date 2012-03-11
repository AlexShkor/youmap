using System.Collections.Generic;
using Telerik.Web.Mvc.Infrastructure;

namespace Jmelosegui.Mvc.Controls.Overlays.Markers.Clustering
{
    public class MarkerClusteringStylesSerializer : IOverlaySerializer
    {
        private readonly MarkerClusteringStyles style;

        public MarkerClusteringStylesSerializer(MarkerClusteringStyles style)
        {
            this.style = style;
        }

        #region IOverlaySerializer Members

        public IDictionary<string, object> Serialize()
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            FluentDictionary.For(result)
                .Add("url", style.Url)
                .Add("height", style.Height)
                .Add("width", style.Width)
                .Add("textSize", style.TextSize)
                .Add("textColor", style.TextColor.ToHtml());

            return result;
        }

        #endregion
    }
}