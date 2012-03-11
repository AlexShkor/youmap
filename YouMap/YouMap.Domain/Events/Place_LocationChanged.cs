using Paralect.Domain;
using YouMap.Domain.Data;

namespace YouMap.Domain.Events
{
    public class Place_LocationChanged : Event
    {
        public string Id { get; set; }

        public Location Location { get; set; }
    }
}