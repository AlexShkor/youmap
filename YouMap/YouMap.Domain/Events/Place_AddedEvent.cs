using System;
using Paralect.Domain;
using YouMap.Domain.Data;

namespace YouMap.Domain
{
    public class Place_AddedEvent: Event
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string CreatorId { get; set; }

        public string Icon { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public Location Location { get; set; }
    }
}