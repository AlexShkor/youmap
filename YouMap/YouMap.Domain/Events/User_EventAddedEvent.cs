using System;
using System.Collections.Generic;
using Paralect.Domain;
using YouMap.Domain.Data;

namespace YouMap.Domain.Events
{
    public class User_EventAddedEvent: Event
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Memo { get; set; }
        public Location Location { get; set; }
        public DateTime Start { get; set; }
        public IEnumerable<string> UsersIds { get; set; }
        public bool Private { get; set; }
        public string PlaceId { get; set; }
        public string OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string UserId { get; set; }
    }
}