using Paralect.ServiceBus;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Domain.Events;
using YouMap.Framework.Environment;

namespace YouMap.EventHandlers
{
    public class FeedDocumentEventHandler : 
        IMessageHandler<User_FeedCreatedEvent>
    {
        private readonly IIdGenerator _idGenerator;
        private readonly FeedDocumentService _documentService;

        public FeedDocumentEventHandler(IIdGenerator idGenerator, FeedDocumentService documentService)
        {
            _idGenerator = idGenerator;
            _documentService = documentService;
        }

        public void Handle(User_FeedCreatedEvent message)
        {
            var doc = new FeedDocument
                          {
                              OwnerId = message.UserId,
                              Id = _idGenerator.Generate(),
                              Name = message.Name
                          };
            _documentService.Insert(doc);
        }
    }
}