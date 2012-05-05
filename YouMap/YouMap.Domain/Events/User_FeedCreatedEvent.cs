using Paralect.Domain;

namespace YouMap.Domain.Events
{
    public class User_FeedCreatedEvent : Event
    {
        public string UserId { get; set; }

        public string Name { get; set; }
    }
}