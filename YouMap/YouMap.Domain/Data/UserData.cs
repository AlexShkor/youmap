using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YouMap.Domain.Enums;

namespace YouMap.Domain.Data
{
    public class UserData
    {
        public string Password { get; set; }

        public string Email { get; set; }

        public VkData Vk { get; set; }

        public IEnumerable<UserPermissionEnum> Permissions { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }
    }
}
