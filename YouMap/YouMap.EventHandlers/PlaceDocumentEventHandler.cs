using MongoDB.Driver.Builders;
using Paralect.ServiceBus;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Domain;

namespace YouMap.EventHandlers
{
    public class PlaceDocumentEventHandler : IMessageHandler<Place_AddedEvent>
    {
        private readonly PlaceDocumentService _documentService;

        public PlaceDocumentEventHandler(PlaceDocumentService documentService)
        {
            _documentService = documentService;
        }

        public void Handle(Place_AddedEvent message)
        {
            var doc = new PlaceDocument
                          {
                              Id = message.Id,
                              CreatorId = message.CreatorId,
                              Location = message.Location,
                              Title = message.Title,
                              Address = message.Address,
                              Description = message.Description,
                              Icon = message.Icon
                          };
            _documentService.Save(doc);
        }
    }
}