using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using YouMap.Domain.Data;

namespace YouMap.Documents.Documents
{
    public class EventDocument
    {
        [BsonId]
        public string Id { get; set; } 
        public string Title { get; set; } 
        public string Memo { get; set; }
        public string PlaceId { get; set; }
        public Location Location { get; set; }
        public DateTime Start { get; set; } 
        public List<string> UsersIds { get; set; } 
        public bool Private { get; set; }
    }
}