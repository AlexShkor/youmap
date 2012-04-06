using MongoDB.Driver;
using MongoDB.Driver.Builders;
using YouMap.Documents.Documents;
using YouMap.Framework;
using YouMap.Framework.Services;

namespace YouMap.Documents.Services
{
    public class EventDocumentService : BaseDocumentService<PlaceDocument, PlaceDocumentFilter>
    {
        public EventDocumentService(MongoRead mongo) : base(mongo)
        {
        }

        protected override MongoCollection Items
        {
            get { throw new System.NotImplementedException(); }
        }

        protected override QueryComplete BuildFilterQuery(PlaceDocumentFilter filter)
        {
            throw new System.NotImplementedException();
        }
    }
}