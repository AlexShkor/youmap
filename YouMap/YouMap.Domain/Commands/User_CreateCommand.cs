using System.Collections.Generic;
using Paralect.Domain;
using Paralect.ServiceBus;
using YouMap.Domain.Enums;
using mPower.Framework;

namespace YouMap.Domain.Commands
{
    public class User_CreateCommand: Command
    {
        public string Password { get; set; }

        public string Email { get; set; }

        public string UserId { get; set; }

        public IEnumerable<UserPermissionEnum> Permissions { get; set; }
    }

    public class User_CreateCommandHandler: CommandHandler<User_CreateCommand>
    {
        public User_CreateCommandHandler(IRepository repository)
            : base(repository)
        {
        }

        public override void Handle(User_CreateCommand message)
        {
            var ar = new UserAR(message.UserId, message.Email, message.Password,message.Metadata);
            Repository.Save(ar);
        }
    }
}