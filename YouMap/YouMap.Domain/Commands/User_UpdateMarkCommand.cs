using Paralect.Domain;
using YouMap.Domain.Data;
using YouMap.Framework;

namespace YouMap.Domain.Commands
{
    public class User_UpdateMarkCommand: Command
    {
        public string UserId { get; set; }
        public Location Location { get; set; }
    }

    public class User_UpdateMarkCommandHandler : CommandHandler<User_UpdateMarkCommand>
    {
        public override void Handle(User_UpdateMarkCommand message)
        {
            var ar = Repository.GetById<UserAR>(message.UserId);
            ar.SetCommandMetadata(message.Metadata);
            ar.UpdateMark(message.Location);
            Repository.Save(ar);
        }
    }
}