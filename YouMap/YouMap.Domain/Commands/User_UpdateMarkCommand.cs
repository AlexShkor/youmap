using System;
using Paralect.Domain;
using YouMap.Domain.Data;
using mPower.Framework;

namespace YouMap.Domain.Commands
{
    public class User_UpdateMarkCommand: Command
    {
        public string UserId { get; set; }
        public Location Location { get; set; }
    }

    public class User_UpdateMarkCommandHandler : CommandHandler<User_UpdateMarkCommand>
    {
        public User_UpdateMarkCommandHandler(IRepository repository) : base(repository)
        {
        }

        public override void Handle(User_UpdateMarkCommand message)
        {
            var ar = Repository.GetById<UserAR>(message.UserId);
            ar.UpdateMark(message.Location);
        }
    }
}