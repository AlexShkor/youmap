using MongoDB.Driver;
using MongoDB.Driver.Builders;
using YouMap.Documents.Documents;
using mPower.Framework;
using mPower.Framework.Services;

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
            return Query.And();
        }
    }

    public class CategoryFilter : BaseFilter
    {
    }
}