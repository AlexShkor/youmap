using System;
using System.Collections.Generic;
using Paralect.Domain;
using YouMap.Domain.Data;
using YouMap.Domain.Enums;

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

        public string CategoryId { get; set; }

        public IEnumerable<DayOfWeek> WorkDays { get; set; }

        public string Logo { get; set; }

        public PlaceStatusEnum Status { get; set; }
    }
}