using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouMap.Domain
{
    public class PlaceData
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string CreatorId { get; set; }

        public string Icon { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public string Address { get; set; }

        public string Description { get; set; }
    }
}
