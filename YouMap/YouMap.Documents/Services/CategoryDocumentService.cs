using MongoDB.Driver;
using MongoDB.Driver.Builders;
using YouMap.Documents.Documents;
using YouMap.Framework;
using YouMap.Framework.Services;

namespace YouMap.Documents.Services
{
    public class CategoryDocumentService: BaseDocumentService<CategoryDocument,CategoryFilter>
    {
        public CategoryDocumentService(MongoRead mongo) : base(mongo)
        {
        }

        protected override MongoCollection Items
        {
            get { return _read.GetCollection("categories"); }
        }

        protected override QueryComplete BuildFilterQuery(CategoryFilter filter)
        {
            var query = Query.And(Query.Null);
            if (filter.IsTop.HasValue)
            {
                query = Query.And(query, Query.EQ("IsTop", filter.IsTop.Value));
            }
            return query;
        }
    }

    public class CategoryFilter : BaseFilter
    {
        public bool? IsTop { get; set; }
    }
}