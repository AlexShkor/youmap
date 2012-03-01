using Paralect.Domain;

namespace YouMap.Domain.Events
{
    public class Place_LocationChanged : Event
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Id { get; set; }
    }
}