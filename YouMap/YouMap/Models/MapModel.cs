using System.Collections.Generic;
using System.Drawing;
using System.Web.Script.Serialization;
using YouMap.Controllers;
using YouMap.Domain.Data;

namespace YouMap.Models
{
    public class MapModel
    {
        public int Height { get; set; }

        public int Width { get; set; }

        public int Zoom { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public IEnumerable<PlaceModel> Places { get; set; }

        public string OpenPopupUrl { get; set; }

        public Location UserLocation { get; set; }

        public Location Defaultlocation { get; set; }

        public string MapUrl { get; set; }

        public bool DisableAutoOpenPlaceInfo { get; set; }

        public MarkerIcon UserIcon { get; set; }

        public MapModel()
        {
            Width = 1450;
            Height = 750;
            Zoom = 12;
            Latitude = 53.90234;
            Longitude = 27.561896;
            Places = new List<PlaceModel>();
        }

        public void ZooomToPlace()
        {
            Zoom = 17;
        }

        public string ToJson()
        {
            var js = new JavaScriptSerializer();
            return js.Serialize(this);
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