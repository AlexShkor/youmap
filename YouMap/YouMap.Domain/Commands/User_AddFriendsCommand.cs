using System.Collections.Generic;
using Paralect.Domain;
using YouMap.Domain.Data;
using YouMap.Framework;

namespace YouMap.Domain.Commands
{
    public class User_AddFriendsCommand: Command
    {
        public string UserId { get; set; }

        public List<Friend> Friends { get; set; }

        public User_AddFriendsCommand()
        {
            Friends = new List<Friend>();
        }
    }

    public class User_AddFriendsCommandHandler: CommandHandler<User_AddFriendsCommand>
    {
        public User_AddFriendsCommandHandler(IRepository repository) : base(repository)
        {
        }

        public override void Handle(User_AddFriendsCommand message)
        {
            var ar = Repository.GetById<UserAR>(message.UserId);
            ar.SetCommandMetadata(message.Metadata);
            ar.AddFriends(message.Friends);
            Repository.Save(ar);
        }
    }
}