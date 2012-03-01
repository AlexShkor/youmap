using System.Drawing;
using Jmelosegui.Mvc.Controls.Overlays.Markers.Clustering;

namespace Jmelosegui.Mvc.Controls.Overlays.Markers
{
    public class MarkerClusteringStyles
    {
        public string Url { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int TextSize { get; set; }
        public Color TextColor { get; set; }

        public IOverlaySerializer CreateSerializer()
        {
            return new MarkerClusteringStylesSerializer(this);
        }
    }
}