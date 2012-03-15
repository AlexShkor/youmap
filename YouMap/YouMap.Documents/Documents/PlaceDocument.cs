using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using YouMap.Domain.Data;
using YouMap.Domain.Enums;

namespace YouMap.Documents.Documents
{
    public class PlaceDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string Title { get; set; }

        public Location Location { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public string CategoryId { get; set; }

        public PlaceStatusEnum Status { get; set; }

        public string OwnerId { get; set; }

        public List<DayOfWeek> WorkDays { get; set; }

        public string Logo { get; set; }
    }
}