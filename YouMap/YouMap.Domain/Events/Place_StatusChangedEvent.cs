using Paralect.Domain;
using YouMap.Domain.Enums;

namespace YouMap.Domain.Events
{
    public class Place_StatusChangedEvent: Event
    {
        public string PlaceId { get; set; }

        public PlaceStatusEnum Status { get; set; }
    }
}