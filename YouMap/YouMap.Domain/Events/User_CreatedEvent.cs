using System.Collections.Generic;
using Paralect.Domain;
using YouMap.Domain.Data;
using YouMap.Domain.Enums;

namespace YouMap.Domain.Events
{
    public class User_CreatedEvent: Event
    {
        public string Password { get; set; }

        public string Email { get; set; }

        public string UserId { get; set; }

        public IEnumerable<UserPermissionEnum> Permissions { get; set; }

        public VkData Vk { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }
    }
}