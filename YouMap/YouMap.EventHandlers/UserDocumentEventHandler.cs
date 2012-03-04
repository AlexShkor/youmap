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
        IMessageHandler<User_ImportedFromVkEvent>
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
            
            if (_documentService.GetById(message.UserId) != null)
            {
                var query = Query.EQ("_id", message.UserId);
                var update = Update.Set("Vk",BsonTypeMapper.MapToBsonValue(message.Vk));
                _documentService.Update(query,update);
            }
            else
            {
                var doc = new UserDocument
                {
                    Id = message.UserId,
                    Vk = message.Vk,
                    FirstName = message.Vk.FirstName,
                    LastName = message.Vk.LastName,
                    UserName = message.Vk.Domain
                };
                _documentService.Save(doc);
            }
        }
    }
}