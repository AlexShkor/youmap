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
                              Password = message.Password
                          };
            _documentService.Save(doc);
        }

        public void Handle(User_ImportedFromVkEvent message)
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