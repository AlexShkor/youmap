using System.Drawing;

namespace Jmelosegui.Mvc.Controls.Overlays
{
    public class MarkerImage
    {
        public MarkerImage(string path, Size size, Point point, Point anchor)
        {
            Path = path;
            Size = size;
            Point = point;
            Anchor = anchor;
        }

        public Point Anchor { get; set; }

        public string Path { get; set; }

        public Point Point { get; set; }

        public Size Size { get; set; }
    }
}
