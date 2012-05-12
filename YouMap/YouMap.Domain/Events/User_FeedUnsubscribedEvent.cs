using Paralect.Domain;

namespace YouMap.Domain
{
    public class User_FeedUnsubscribedEvent : Event
    {
        public string UserId { get; set; }

        public string Feed { get; set; }
    }
}