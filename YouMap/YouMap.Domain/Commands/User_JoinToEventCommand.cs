﻿using Paralect.Domain;
using YouMap.Framework;

namespace YouMap.Domain.Commands
{
    public class User_JoinToEventCommand: Command
    {
        public string NewMemberId { get; set; }

        public string UserId { get; set; }

        public string EventId { get; set; }
    }

    public class User_JoinToEventCommandHandler: CommandHandler<User_JoinToEventCommand>
    {
        public User_JoinToEventCommandHandler(IRepository repository) : base(repository)
        {
        }

        public override void Handle(User_JoinToEventCommand message)
        {
            var ar = Repository.GetById<UserAR>(message.UserId);
            ar.SetCommandMetadata(message.Metadata);
            ar.AddMemberToEvent(message.NewMemberId,message.EventId);
            Repository.Save(ar);
        }
    }
}