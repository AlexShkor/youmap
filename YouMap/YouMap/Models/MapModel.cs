using System.Collections.Generic;
using System.Drawing;
using YouMap.Controllers;

namespace YouMap.Models
{
    public class MapModel
    {
        public int Height { get; set; }

        public int Width { get; set; }

        public int Zoom { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public IEnumerable<PlaceModel> Markers { get; set; }

        public MarkerIcon IconShadow { get; set; }

        public MapModel()
        {
            Width = 1450;
            Height = 750;
            Zoom = 12;
            Latitude = 53.90234;
            Longitude = 27.561896;
            Markers = new List<PlaceModel>();
        }
    }

    public class MarkerIcon
    {
        public string Path { get; set; }

        public Size Size { get; set; }

        public Point Point { get; set; }

        public Point Anchor { get; set; }
    }
}