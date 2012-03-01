using Paralect.Domain;

namespace YouMap.Domain
{
    public class Place_AddedEvent: Event
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string CreatorId { get; set; }

        public string Icon { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }
    }
}