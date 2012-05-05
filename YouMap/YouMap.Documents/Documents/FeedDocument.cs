using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace YouMap.Documents.Documents
{
    public class FeedDocument
    {
        [BsonId]
        public String Id { get; set; }
        public String Name { get; set; }
        public String OwnerId { get; set; }
    }
}
