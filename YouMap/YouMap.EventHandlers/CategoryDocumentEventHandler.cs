using MongoDB.Driver.Builders;
using Paralect.ServiceBus;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Domain;
using YouMap.Domain.Events;

namespace YouMap.EventHandlers
{
    public class CategoryDocumentEventHandler: 
        IMessageHandler<Category_CreatedEvent>,
        IMessageHandler<Category_UpdatedEvent>,
        IMessageHandler<Category_DeletedEvent>
    {
        private readonly CategoryDocumentService _documentService;

        public CategoryDocumentEventHandler(CategoryDocumentService documentService)
        {
            _documentService = documentService;
        }

        public void Handle(Category_CreatedEvent message)
        {
            var doc = new CategoryDocument
                          {
                              Id = message.Id,
                              Name = message.Name,
                              Icon = message.Icon,
                              IsTop = message.IsTop
                          };
            _documentService.Save(doc);
        }

        public void Handle(Category_UpdatedEvent message)
        {
            var query = Query.EQ("_id", message.Id);
            var update = Update.Set("Name", message.Name)
                        .Set("Icon", message.Icon)
                        .Set("IsTop", message.IsTop);
            _documentService.Update(query,update);
        }

        public void Handle(Category_DeletedEvent message)
        {
            _documentService.RemoveById(message.Id);
        }
    }
}