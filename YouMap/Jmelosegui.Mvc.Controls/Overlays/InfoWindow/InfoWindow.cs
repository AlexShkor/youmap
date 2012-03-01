using System.Web.Script.Serialization;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.Extensions;
using Telerik.Web.Mvc.Infrastructure;
using Telerik.Web.Mvc.UI;

namespace Jmelosegui.Mvc.Controls.Overlays
{
    public class InfoWindow : Overlay, IHideObjectMembers
    {
        
        private readonly Marker marker;
        private readonly string content;
        public InfoWindow(Marker marker) : this(marker.Map)
        {
            Guard.IsNotNull(marker, "marker");
            this.marker = marker;
            content = "{0}Marker{1}".FormatWith(marker.Map.Id,marker.Index);
            Template = new HtmlTemplate();
        }

        public InfoWindow(GoogleMap map) : base(map)
        {
            Guard.IsNotNull(map, "GoogleMap");
        }

        [ScriptIgnore]
        public HtmlTemplate Template
        {
            get;
            private set;
        }

        public string Content { get { return content; } }

        public bool DisableAutoPan { get; set; }

        public override double Longitude
        {
            get
            {
                if (marker != null)
                    return marker.Longitude;

                return base.Longitude;
            }
            set { base.Longitude = value; }
        }

        public override double Latitude
        {
            get
            {
                if (marker != null)
                    return marker.Latitude;

                return base.Latitude;
            }
            set { base.Latitude = value; }
        }

        public int MaxWidth { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "z")]
        public int zIndex { get; set; }
    }
}