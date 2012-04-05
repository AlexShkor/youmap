using System.Collections.Generic;
using Paralect.Domain;
using YouMap.Domain.Data;

namespace YouMap.Domain.Events
{
    public class User_FriendsAddedEvent: Event
    {
        public string UserId { get; set; }

        public List<Friend> Friends { get; set; }
    }
}