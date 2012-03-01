namespace YouMap.Controllers
{
    public class PlaceModel
    {
        public string Title { get; set; }

        public string Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Description { get; set; }

        public string Icon { get; set; }

        public bool Draggable { get; set; }

        public string Id { get; set; }
    }
}