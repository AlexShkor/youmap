using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using YouMap.Documents.Documents;
using YouMap.Framework;
using YouMap.Framework.Services;
using YouMap.Framework.Utils.Extensions;

namespace YouMap.Documents.Services
{
    public class UserDocumentService : BaseDocumentService<UserDocument, UserFilter>
    {
        public UserDocumentService(MongoRead mongo)
            : base(mongo)
        {
        }

        protected override MongoCollection Items
        {
            get { return _read.Users; }
        }

        protected override QueryComplete BuildFilterQuery(UserFilter filter)
        {
            var query = Query.And(Query.Null);
            if (filter.UserName.HasValue())
            {
                query = Query.And(query, Query.EQ("UserName", filter.UserName));
            }
            else if (filter.UserNameOrEmail.HasValue())
            {
                query = Query.And(query,
                                  Query.Or(Query.EQ("UserName", filter.UserNameOrEmail),
                                           Query.EQ("Email", filter.UserNameOrEmail)));
            }
            if (!string.IsNullOrEmpty(filter.UserId))
            {
                query = Query.And(query, Query.EQ("_id", filter.UserId));
            }
            if (filter.Email.HasValue())
            {
                query = Query.And(query, Query.EQ("Email", filter.Email));
            }
            if (filter.IsActive.HasValue)
            {
                query = Query.And(query, Query.EQ("IsActive", filter.IsActive.Value));
            }
            if (!string.IsNullOrEmpty(filter.VkId))
            {
                query = Query.And(query, Query.EQ("Vk.Id", filter.VkId));
            }
            if (filter.HasPermission.HasValue)
            {
                query = Query.And(query, Query.EQ("Permissions", filter.HasPermission.Value));
            }
            if (filter.VkIdIn != null)
            {
                query = Query.And(query, Query.In("Vk.Id", BsonArray.Create(filter.VkIdIn)));
            }
            if (filter.LastLocationDateGreaterThan.HasValue)
            {
                query = Query.And(query, Query.GTE("LocatedDate", filter.LastLocationDateGreaterThan.Value));
            }
            if (filter.EventsForUserWithId.HasValue())
            {
                query = Query.And(Query.EQ("Friends",filter.EventsForUserWithId),Query.Or(Query.EQ("Events.Private", false),
                                  Query.EQ("Events.UsersIds", filter.EventsForUserWithId)));
            }
            if (filter.EventInPlace.HasValue())
            {
                query = Query.And(Query.EQ("Events.PlaceId",filter.EventInPlace));
            }
            if (filter.CheckInPlace.HasValue())
            {
                query = Query.And(Query.EQ("CheckIns.PlaceId", filter.CheckInPlace));
            }
            if (filter.EventIdEq.HasValue())
            {
                query = Query.And(Query.EQ("Events._id", filter.EventIdEq));
            }
            return query;
        }

        public IOrderedEnumerable<EventDocument> GetEventsListForPlace(string placeId, int count)
        {
            var users = GetByFilter(new UserFilter
                                        {
                                            EventInPlace = placeId
                                        });
            var events = users.SelectMany(x => x.Events).Where(
                                       x =>
                                       x.PlaceId == placeId).ToList();
            var date = DateTime.Now;
            var prev = events.Where(x => x.Start < date).OrderByDescending(
                x => x.Start).Take(count);
            var next = events.Where(x => x.Start >= date).OrderBy(x => x.Start).Take(count);
            return prev.Concat(next).OrderByDescending(x => x.Start);
        }

        public IOrderedEnumerable<CheckInDocument> GetCheckInsListForPlace(string placeId, int count)
        {
            var users = GetByFilter(new UserFilter
                                        {
                                            CheckInPlace = placeId
                                        });
            return CheckInsForUsers(users, placeId, count);
        }

        private static IOrderedEnumerable<CheckInDocument> CheckInsForUsers(IEnumerable<UserDocument> users, string placeId, int count)
        {
            var events = users.SelectMany(x => x.CheckIns).Where(
                x =>
                x.PlaceId == placeId).ToList();
            var date = DateTime.Now;
            var prev = events.Where(x => x.Visited < date).OrderByDescending(
                x => x.Visited).Take(count);
            var next = events.Where(x => x.Visited >= date).OrderBy(x => x.Visited).Take(count);
            return prev.Concat(next).OrderByDescending(x => x.Visited);
        }

        public IEnumerable<IGrouping<UserDocument, CheckInDocument>> GetCheckInsGroupsForPlace(string placeId, int count)
        {
            var users = GetByFilter(new UserFilter
                                        {
                                            CheckInPlace = placeId
                                        });
            var item = CheckInsForUsers(users, placeId, count);
            return item.GroupBy(c => users.First(x => x.CheckIns.Contains(c)));
        }

        public EventDocument GetEventById(string eventid)
        {
            return GetByFilter(new UserFilter {EventIdEq = eventid}).SelectMany(x => x.Events).First(x => x.Id == eventid);
        }
    }

    public class UserFilter : BaseFilter
    {

        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public bool? IsActive { get; set; }

        public string UserNameOrEmail { get; set; }

        public string VkId { get; set; }

        public Domain.Enums.UserPermissionEnum? HasPermission { get; set; }

        public IEnumerable<String> VkIdIn { get; set; }

        public DateTime? LastLocationDateGreaterThan { get; set; }

        public string EventsForUserWithId { get; set; }

        public string IdOrVkIdEqual { get; set; }

        public string EventInPlace { get; set; }

        public string CheckInPlace { get; set; }

        public string EventIdEq { get; set; }
    }
}