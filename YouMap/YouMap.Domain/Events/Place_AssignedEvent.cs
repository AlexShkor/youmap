using Paralect.Domain;

namespace YouMap.Domain.Events
{
    public class Place_AssignedEvent: Event
    {
        public string PlaceId { get; set; }

        public string OwnerId { get; set; }
    }
}