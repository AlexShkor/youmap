using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using YouMap.Documents.Documents;
using YouMap.Framework;
using YouMap.Framework.Services;

namespace YouMap.Documents.Services
{
    public class FeedDocumentService: BaseDocumentService<FeedDocument,BaseFilter>
    {
        public FeedDocumentService(MongoRead mongo) : base(mongo)
        {
        }

        protected override MongoCollection Items
        {
            get { return _read.GetCollection("feeds"); }
        }

        protected override QueryComplete BuildFilterQuery(BaseFilter filter)
        {
            return Query.And();
        }

        public FeedDocument GetByName(string name)
        {
            return GetByQuery(Query.EQ("Name", name)).FirstOrDefault();
        }
    }
}