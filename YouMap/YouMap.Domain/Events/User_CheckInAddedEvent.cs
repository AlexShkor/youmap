using Paralect.Domain;
using YouMap.Domain.Data;

namespace YouMap.Domain.Events
{
    public class User_CheckInAddedEvent: Event
    {
        public string Memo { get; set; }

        public string Title { get; set; }

        public Location Location { get; set; }

        public string PlaceId { get; set; }

        public string UserId { get; set; }

        public DateTime Visited { get; set; }
    }
}