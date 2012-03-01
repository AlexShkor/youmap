using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Framework.Services;

namespace mPower.Framework.Mongo
{
    public class NLogMongoService : BaseMongoService<NLogMongoTarget.NlogMongoItem, NLogMongoFilter>
    {
        private readonly string _connectionString;
        private readonly string _collectionName;

        public NLogMongoService(string connectionString, string collectionName)
        {
            _connectionString = connectionString;
            _collectionName = collectionName;
        }

        protected override MongoCollection Items
        {
            get
            {
                var url = MongoUrl.Create(_connectionString);
                var dbName = url.DatabaseName;
                var server = MongoServer.Create(_connectionString);
                return server.GetDatabase(dbName).GetCollection(_collectionName);
            }
        }

        protected override QueryComplete BuildFilterQuery(NLogMongoFilter filter)
        {
            var query = Query.And();
            // Search
            if (!String.IsNullOrEmpty(filter.SearchKey))
            {
                 query = Query.And(query,Query.Or(Query.EQ("_id", filter.SearchKey), Query.EQ("UserId", filter.SearchKey),
                                     Query.EQ("UserEmail", filter.SearchKey)));
                
            }
            // Date
            if (filter.MinDate != null )
            {
                query = Query.And(query, Query.GTE("Date", filter.MinDate.Value));
            }

            if (filter.MaxDate != null)
            {
                query = Query.And(query, Query.LTE("Date", filter.MaxDate.Value));
            }

            // Level
            if (!String.IsNullOrEmpty(filter.Level))
            {
                query = Query.And(query, Query.EQ("Level", filter.Level));
            }

            return query;
        }

        protected override IMongoSortBy BuildSortExpression(NLogMongoFilter filter)
        {
            if (!String.IsNullOrEmpty(filter.SortExpression))
            {
                return SortBy.Descending(filter.SortExpression);
            }
            return SortBy.Null;
        }
    }

    public class NLogMongoFilter : BaseFilter
    {
        public string SearchKey { get; set; }

        public DateTime? MaxDate { get; set; }

        public DateTime? MinDate { get; set; }

        public string Level { get; set; }

        public String SortExpression { get; set; }
    }
}
