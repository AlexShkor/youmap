namespace YouMap.Models
{
    public class PlaceModel
    {
        public string Title { get; set; }

        public string Address { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public string CategoryId { get; set; }

        public string Description { get; set; }

        public MarkerIcon Icon { get; set; }

        public bool Draggable { get; set; }

        public string Id { get; set; }

        public string InfoWindowUrl { get; set; }

        public bool OpenOnLoad { get; set; }

        public MarkerIcon Shadow { get; set; }

        public PlaceModel()
        {
            Icon = new MarkerIcon();
            Shadow = new MarkerIcon();
        }
    }
}