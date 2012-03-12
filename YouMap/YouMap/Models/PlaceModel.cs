namespace YouMap.Models
{
    public class PlaceModel
    {
        public string Title { get; set; }

        public string Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Description { get; set; }

        public MarkerIcon Icon { get; set; }

        public bool Draggable { get; set; }

        public string Id { get; set; }

        public string InfoWindowUrl { get; set; }

        public PlaceModel()
        {
            Icon =new MarkerIcon();
        }
    }
}