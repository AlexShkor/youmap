using Telerik.Web.Mvc.Infrastructure;

namespace Jmelosegui.Mvc.Controls.Overlays
{
    public class MarkerSerializer : IOverlaySerializer
    {
        private readonly Marker marker;

        public MarkerSerializer(Marker marker)
        {
            this.marker = marker;
        }

        public virtual IDictionary<string, object> Serialize()
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            FluentDictionary.For(result)
                .Add("Title", marker.Title, () => marker.Title != null)
                .Add("Latitude", marker.Latitude)
                .Add("Longitude", marker.Longitude)
                .Add("zIndex", marker.zIndex)
                .Add("Clickable", marker.Clickable, () => marker.Clickable)
                .Add("Draggable", marker.Draggable, () => marker.Draggable)
                .Add("Icon", marker.Icon, () => marker.Icon != null)
                .Add("Shadow", marker.Shadow, () => marker.Shadow != null)
                .Add("Window", marker.Window, () => marker.Window != null);
                

            return result;
        }
    }
}
