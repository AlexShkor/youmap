using MongoDB.Bson.Serialization.Attributes;
using YouMap.Domain.Data;

namespace YouMap.Documents.Documents
{
    public class PlaceDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string Title { get; set; }

        public string CreatorId { get; set; }

        public Location Location { get; set; }

        public string Icon { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public string CategoryId { get; set; }
    }
}