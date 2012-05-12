using Paralect.Domain;
using YouMap.Framework;

namespace YouMap.Domain.Commands
{
    public class User_UnsubscribeFeedCommand : Command
    {
        public string UserId { get; set; }
        public string Feed { get; set; }
    }

    public class User_UnsubscribeFeedCommandHandler : CommandHandler<User_UnsubscribeFeedCommand>
    {
        public override void Handle(User_UnsubscribeFeedCommand message)
        {
            var ar = Repository.GetById<UserAR>(message.UserId);
            ar.SetCommandMetadata(message.Metadata);
            ar.UnsubscribeFeed(message.Feed);
            Repository.Save(ar);
        }
    }
}