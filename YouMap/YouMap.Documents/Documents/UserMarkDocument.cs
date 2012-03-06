using System;
using MongoDB.Bson.Serialization.Attributes;
using YouMap.Domain.Data;

namespace YouMap.Documents.Documents
{
    public class UserMarkDocument
    {
        [BsonId]
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTime Visited { get; set; }
        public Location Location { get; set; }
    }
}