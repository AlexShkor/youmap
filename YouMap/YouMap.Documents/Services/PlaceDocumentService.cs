using System;
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
            if (filter.PlaceId.HasValue())
            {
                query = Query.And(query, Query.EQ("_id", filter.PlaceId));
            }
            if (filter.CategoryId.HasValue())
            {
                query = Query.And(query, Query.EQ("CategoryId", filter.CategoryId));
            }
            if (filter.OwnerId.HasValue())
            {
                query = Query.And(query, Query.EQ("OwnerId", filter.OwnerId));
            }
            if (filter.StatusEq.HasValue)
            {
                query = Query.And(query, Query.EQ("Status", filter.StatusEq.Value));
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

        public IEnumerable<IGrouping<double, PlaceDocument>> GetPlacesForLocation(Location location, int count = 100)
        {
            return GetNear(location, count, 0.8);
        }

        public IEnumerable<IGrouping<double, PlaceDocument>> GetNear(Location location, int count = 100, double radiusInKm = 1)
        {
            
            var query = Query.EQ("Status", PlaceStatusEnum.Active);
            //TODO: FIX  distance calculation!

            var options = GeoNearOptions.SetMaxDistance(radiusInKm/EarthRadius).SetSpherical(true);
            var result = Items.GeoNearAs<PlaceDocument>(query,
                                                        location.Latitude,
                                                        location.Longitude, //incorrect ordering until database regeneration will be done
                                                        count,
                                                        options);
            return result.Hits.GroupBy(x => x.Distance * EarthRadius,y=> y.Document);
        }

        public PlaceDocument GetPlaceForLocation(Location location)
        {
            try
            {
                return GetPlacesForLocation(location, 1).First().First();
            }
            catch (Exception)
            {
                return null;
            }
            
        }
    }

    public class PlaceDocumentFilter : BaseFilter
    {
        public string CategoryId { get; set; }

        public Location Location { get; set; }

        public string OwnerId { get; set; }

        public IEnumerable<string> IdIn { get; set; }

        public List<PlaceStatusEnum> StatusNotIn { get; set; }

        public string PlaceId { get; set; }

        public PlaceStatusEnum? StatusEq { get; set; }

        public PlaceDocumentFilter()
        {
            StatusNotIn = new List<PlaceStatusEnum>();
        }
    }
}