using Paralect.Domain;

namespace YouMap.Domain.Events
{
    public class User_FeedSubscribedEvent : Event
    {
        public string UserId { get; set; }

        public string Feed { get; set; }
    }
}