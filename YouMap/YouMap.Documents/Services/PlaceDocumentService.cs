using System;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using YouMap.Documents.Documents;
using mPower.Framework;
using mPower.Framework.Services;

namespace YouMap.Documents.Services
{
    public class PlaceDocumentService: BaseDocumentService<PlaceDocument,PlaceDocumentFilter>
    {
        public PlaceDocumentService(MongoRead mongo) : base(mongo)
        {
        }

        protected override MongoCollection Items
        {
            get { return _read.GetCollection("places"); }
        }

        protected override QueryComplete BuildFilterQuery(PlaceDocumentFilter filter)
        {
            var query = Query.And();
            if (filter.CategoryId.HasValue())
            {
                query = Query.And(query, Query.EQ("CategoryId", filter.CategoryId));
            }
            return query;
        }
    }

    public class PlaceDocumentFilter : BaseFilter
    {
        public string CategoryId { get; set; }
    }
}