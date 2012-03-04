using System.Collections.Generic;
using Paralect.Domain;
using Paralect.ServiceBus;
using YouMap.Domain.Enums;
using mPower.Framework;
using YouMap.Domain.Data;

namespace YouMap.Domain.Commands
{
    public class User_CreateCommand: Command
    {
        public string Password { get; set; }

        public string Email { get; set; }

        public string UserId { get; set; }

        public VkData Vk { get; set; }

        public IEnumerable<UserPermissionEnum> Permissions { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }
    }

    public class User_CreateCommandHandler: CommandHandler<User_CreateCommand>
    {
        public User_CreateCommandHandler(IRepository repository)
            : base(repository)
        {
        }

        public override void Handle(User_CreateCommand message)
        {
            var ar = new UserAR(message.UserId, new UserData
            {
                FirstName = message.FirstName,
                LastName = message.LastName,
                UserName = message.UserName,
                Email = message.Email,
                Password = message.Password,
                Permissions = message.Permissions,
                Vk = message.Vk
            }, message.Metadata);
            Repository.Save(ar);
        }
    }
}