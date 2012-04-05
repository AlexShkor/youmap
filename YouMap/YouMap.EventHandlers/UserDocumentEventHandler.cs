﻿using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Paralect.ServiceBus;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Domain;
using YouMap.Domain.Events;

namespace YouMap.EventHandlers
{
    public class UserDocumentEventHandler: 
        IMessageHandler<User_CreatedEvent>,
        IMessageHandler<User_ImportedFromVkEvent>,
        IMessageHandler<User_MarkUdatedEvent>,
        IMessageHandler<User_EventAddedEvent>,
        IMessageHandler<User_EventMemberAddedEvent>,
        IMessageHandler<User_FriendsAddedEvent>,
        IMessageHandler<User_CheckInAddedEvent>
    {
        private readonly UserDocumentService _documentService;

        public UserDocumentEventHandler(UserDocumentService userDocumentService)
        {
            _documentService = userDocumentService;
        }

        public void Handle(User_CreatedEvent message)
        {
            var doc = new UserDocument
                          {
                              Id = message.UserId,
                              Email = message.Email,
                              Password = message.Password,
                              Permissions =  message.Permissions,
                              Vk = message.Vk,
                              UserName = message.UserName,
                              LastName = message.LastName,
                              FirstName = message.FirstName
                          };
            _documentService.Save(doc);
        }

        public void Handle(User_ImportedFromVkEvent message)
        {

            var query = Query.EQ("_id", message.UserId);
            var update = Update.Set("Vk", BsonTypeMapper.MapToBsonValue(message.Vk));
            _documentService.Update(query, update);
        }

        public void Handle(User_MarkUdatedEvent message)
        {
            var query = Query.EQ("_id", message.UserId);
            var mark = new UserMarkDocument
                           {
                               Location = message.Location,
                               Visited = message.Visited
                           };
            var update = Update.Set("LastMark", mark.ToBsonDocument());
            _documentService.Update(query,update);
        }

        public void Handle(User_CheckInAddedEvent message)
        {
            var query = Query.EQ("_id", message.UserId);
            var checkIn = new CheckInDocument
                              {
                                  Location = message.Location,
                                  Memo = message.Memo,
                                  Title = message.Title,
                                  Visited = message.Visited,
                                  PlaceId = message.PlaceId
                              };
            var update = Update.PushWrapped("CheckIns", checkIn);
            _documentService.Update(query,update);
        }

        public void Handle(User_EventAddedEvent message)
        {
            var query = Query.EQ("_id", message.OwnerId);
            var doc = new EventDocument
                          {
                              Id = message.Id,
                              Location = message.Location,
                              Memo = message.Memo,
                              PlaceId = message.PlaceId,
                              Private = message.Private,
                              Start = message.Start,
                              Title = message.Title,
                              UsersIds = message.UsersIds.ToList()
                          };
            var update = Update.PushWrapped("Events", doc);
            _documentService.Update(query,update);
        }

        public void Handle(User_EventMemberAddedEvent message)
        {
            var query = Query.And(Query.EQ("_id", message.UserId), Query.EQ("Events._id",message.EventId));
            var update = Update.AddToSet("Events.$.UsersIds", message.NewMemberId);
            _documentService.Update(query,update);
        }

        public void Handle(User_FriendsAddedEvent message)
        {
            var query = Query.EQ("_id", message.UserId);
            var update = Update.PushWrapped("Friends", message.Friends.Select(x => x.VkId));
            _documentService.Update(query,update);
        }
    }
}