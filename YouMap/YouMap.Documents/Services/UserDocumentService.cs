using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using YouMap.Documents.Documents;
using YouMap.Framework;
using YouMap.Framework.Services;

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
            if (!string.IsNullOrEmpty(filter.UserName))
            {
                query = Query.And(query, Query.EQ("UserName", filter.UserName));
            }
            else if (!string.IsNullOrEmpty(filter.UserNameOrEmail))
            {
                query = Query.And(query,
                                  Query.Or(Query.EQ("UserName", filter.UserNameOrEmail),
                                           Query.EQ("Email", filter.UserNameOrEmail)));
            }
            if (!string.IsNullOrEmpty(filter.UserId))
            {
                query = Query.And(query, Query.EQ("_id", filter.UserId));
            }
            if (!string.IsNullOrEmpty(filter.Email))
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
            return query;
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
    }
}