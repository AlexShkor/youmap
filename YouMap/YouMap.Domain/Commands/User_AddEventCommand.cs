﻿using System;
using System.Collections.Generic;
using Paralect.Domain;
using YouMap.Domain.Data;
using YouMap.Framework;

namespace YouMap.Domain.Commands
{
    public class User_AddEventCommand : Command, IEventData
    {
        public string EventId { get; set; }
        public string Title { get; set; }
        public string Memo { get; set; }
        public Location Location { get; set; }
        public DateTime Start { get; set; }
        public bool Private { get; set; }
        public string PlaceId { get; set; }
        public string OwnerId { get; set; }
        public IEnumerable<Friend> Members { get; set; }
    }

    public class User_CreateEventCommandHandler: CommandHandler<User_AddEventCommand>
    {
        public override void Handle(User_AddEventCommand message)
        {
            var ar = Repository.GetById<UserAR>(message.OwnerId);
            ar.SetCommandMetadata(message.Metadata);
            ar.AddEvent(message);
            Repository.Save(ar);
        }
    }
}