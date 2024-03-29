﻿using System;
using MongoDB.Bson.Serialization.Attributes;
using YouMap.Domain.Data;

namespace YouMap.Documents.Documents
{
    [BsonIgnoreExtraElements]
    public class UserMarkDocument
    {
        public string UserId { get; set; }
        public DateTime Visited { get; set; }
        public Location Location { get; set; }
    }
}