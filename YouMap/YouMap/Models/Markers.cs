namespace YouMap.Models
{
    public class PlaceModel: MarkerModel
    {
        public string CategoryId { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public string Id { get; set; }
    }

    public class FriendMarkerModel: MarkerModel
    {
        public string Id { get; set; }

        public string Visited { get; set; }
    }

    public class EventMarkerModel: MarkerModel
    {
        public string PlaceId { get; set; }
    }

    public class MarkerModel
    {
        public string Title { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public MarkerIcon Shadow { get; set; }

        public string InfoWindowUrl { get; set; }

        public bool OpenOnLoad { get; set; }

        public MarkerIcon Icon { get; set; }

        public bool Draggable { get; set; }

        public MarkerModel()
        {
            Icon = new MarkerIcon();
            Shadow = new MarkerIcon();
        }
    }
}