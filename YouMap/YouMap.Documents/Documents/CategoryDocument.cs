using MongoDB.Bson.Serialization.Attributes;

namespace YouMap.Documents.Documents
{
    public class CategoryDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public bool IsTop { get; set; }
    }
}