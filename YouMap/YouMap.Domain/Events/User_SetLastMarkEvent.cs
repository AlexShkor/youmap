using System;
using Paralect.Domain;
using YouMap.Domain.Data;

namespace YouMap.Domain.Events
{
    public class User_MarkUdatedEvent: Event
    {
        public string UserId { get; set; } 
        public DateTime Visited { get; set; }
        public Location Location { get; set; }
    }
}