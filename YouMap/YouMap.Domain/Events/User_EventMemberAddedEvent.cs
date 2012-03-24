using MongoDB.Bson;
using Paralect.Domain;

namespace YouMap.Domain.Events
{
    public class User_EventMemberAddedEvent: Event
    {
        public string EventId { get; set; }
        public string UserId { get; set; }
        public string NewMemberId { get; set; }
    }
}