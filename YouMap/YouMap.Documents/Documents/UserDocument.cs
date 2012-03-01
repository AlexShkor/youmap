using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using YouMap.Domain.Data;
using mPower.Framework;

namespace YouMap.Documents.Documents
{
    public class UserDocument: IUserIdentity
    {
        [BsonId]
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                return String.Format("{0} {1}", FirstName, LastName);
            }
        }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string AuthToken { get; set; }

        public string MobileAccessToken { get; set; }

        public VkData Vk { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<UserPermissionEnum> Permissions { get; set; }

        public bool HasPermissions(params UserPermissionEnum[] permissions)
        {
            return permissions.All(permission => Permissions.Contains(permission));
        }
    }

    public enum UserPermissionEnum
    {
        User,
        Admin
    }
}