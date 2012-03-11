﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YouMap.Domain.Data;

namespace YouMap.Domain
{
    public class PlaceData
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string CreatorId { get; set; }

        public string Address { get; set; }

        public string Description { get; set; }

        public Location Location { get; set; }

        public IEnumerable<DayOfWeek> WorkDays { get; set; }

        public string CategoryId { get; set; }
    }
}
