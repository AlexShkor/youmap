﻿using System;
using System.Collections.Generic;
using Paralect.Domain;
using YouMap.Domain.Data;

namespace YouMap.Domain.Events
{
    public class Place_UpdatedEvent: Event
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

        public int Layer { get; set; }

        public List<string> Tags { get; set; } 
    }
}