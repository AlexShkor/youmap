using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using YouMap.Documents.Documents;
using YouMap.Domain.Data;
using YouMap.Domain.Enums;
using YouMap.Framework;
using YouMap.Framework.Services;
using YouMap.Framework.Utils.Extensions;

namespace YouMap.Documents.Services
{
    public class PlaceDocumentService: BaseDocumentService<PlaceDocument,PlaceDocumentFilter>
    {
        private const double EarthRadius = 6378.0; // km

        public PlaceDocumentService(MongoRead mongo) : base(mongo)
        {
        }

        protected override MongoCollection Items
        {
            get { return _read.GetCollection("places"); }
        }

        protected override QueryComplete BuildFilterQuery(PlaceDocumentFilter filter)
        {
            var query = Query.And(Query.Null);
            if (filter.CategoryId.HasValue())
            {
                query = Query.And(query, Query.EQ("CategoryId", filter.CategoryId));
            }
            if (filter.OwnerId.HasValue())
            {
                query = Query.And(query, Query.EQ("OwnerId", filter.OwnerId));
            }
            if (filter.Location != null)
            {
                query = Query.And(query, Query.EQ("Location", BsonValue.Create(filter.Location)));
            }
            if (filter.IdIn != null && filter.IdIn.Any())
            {
                query = Query.And(query, Query.In("_id", BsonArray.Create(filter.IdIn)));
            }
            if (filter.StatusNotIn != null && filter.StatusNotIn.Any())
            {
                query = Query.And(query, Query.NotIn("Status", BsonArray.Create(filter.StatusNotIn)));
            }
            return query;
        }

        public IEnumerable<PlaceDocument> GetPlacesForLocation(Location location, int count = 100)
        {
            return GetNear(location, count, 0.01);
        }

        public IEnumerable<PlaceDocument> GetNear(Location location, int count = 100, double radiusInKm = 1)
        {
            var near = Query.And();

            var options = GeoNearOptions
                .SetMaxDistance(radiusInKm / EarthRadius)
                .SetSpherical(true);
            var result = Items.GeoNearAs<PlaceDocument>(near,
                                                        location.Longitude,
                                                        location.Latitude,
                                                        count,
                                                        options
                );
            return result.Response.Cast<PlaceDocument>();
        }

        public PlaceDocument GetPlaceForLocation(Location location)
        {
            return GetPlacesForLocation(location, 1).SingleOrDefault();
        }
    }

    public class PlaceDocumentFilter : BaseFilter
    {
        public string CategoryId { get; set; }

        public Location Location { get; set; }

        public string OwnerId { get; set; }

        public IEnumerable<string> IdIn { get; set; }

        public List<PlaceStatusEnum> StatusNotIn { get; set; }

        public PlaceDocumentFilter()
        {
            StatusNotIn = new List<PlaceStatusEnum>();
        }
    }
}