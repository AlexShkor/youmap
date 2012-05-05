using Paralect.Domain;
using YouMap.Framework;

namespace YouMap.Domain.Commands
{
    public class User_CreateFeedCommand: Command
    {
        public string UserId { get; set; }

        public string Name { get; set; }
    }

    public class User_CreateFeedCommandHandler: CommandHandler<User_CreateFeedCommand>
    {
        public override void Handle(User_CreateFeedCommand message)
        {
            var ar = Repository.GetById<UserAR>(message.UserId);
            ar.SetCommandMetadata(message.Metadata);
            ar.CreateFeed(message.Name);
            Repository.Save(ar);
        }
    }
}