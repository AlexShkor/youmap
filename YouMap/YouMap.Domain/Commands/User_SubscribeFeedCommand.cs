using Paralect.Domain;
using YouMap.Framework;

namespace YouMap.Domain.Commands
{
    public class User_SubscribeFeedCommand: Command
    {
        public string UserId { get; set; }
        public string Feed { get; set; }
    }


    public class User_SubscribeFeedCommandHandler: CommandHandler<User_SubscribeFeedCommand>
    {
        public override void Handle(User_SubscribeFeedCommand message)
        {
            var ar = Repository.GetById<UserAR>(message.UserId);
            ar.SetCommandMetadata(message.Metadata);
            ar.SubscribeFeed(message.Feed);
            Repository.Save(ar);
        }
    }
}