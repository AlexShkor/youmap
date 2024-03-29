﻿using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using YouMap.Domain.Auth;
using YouMap.Domain.Data;
using YouMap.Domain.Enums;

namespace YouMap.Documents.Documents
{
    public class UserDocument: IUserIdentity
    {
        [BsonId]
        public string Id { get; set; }

        public string VkId
        {
            get { return Vk != null ? Vk.Id : null; }
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                if (FirstName == null && LastName == null)
                {
                    return null;
                }
                return String.Format("{0} {1}", FirstName, LastName);
            }
        }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string AuthToken { get; set; }

        public string MobileAccessToken { get; set; }

        public UserMarkDocument LastMark { get; set; }

        public VkData Vk { get; set; }

        public bool IsActive { get; set; }

        public DateTime? LastCheckInDate { get; set; }

        public List<CheckInDocument> CheckIns { get; set; }

        public List<EventDocument> Events { get; set; }

        public List<String> Feeds { get; set; }

        public HashSet<string> Friends { get; set; } 

        public string Name
        {
            get { return FullName; }
        }

        public IEnumerable<UserPermissionEnum> Permissions { get; set; }

        public bool HasPermissions(params UserPermissionEnum[] permissions)
        {
            return permissions.All(permission => Permissions.Contains(permission));
        }

        public UserDocument()
        {
            CheckIns = new List<CheckInDocument>();
            Permissions = new List<UserPermissionEnum>();
            Friends = new HashSet<string>();
            Events = new List<EventDocument>();
            Feeds = new List<string>();
        }
    }

   
}