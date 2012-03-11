using MongoDB.Driver.Builders;
using Paralect.ServiceBus;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Domain;
using YouMap.Domain.Events;

namespace YouMap.EventHandlers
{
    public class PlaceDocumentEventHandler :
        IMessageHandler<Place_AddedEvent>,
        IMessageHandler<Place_StatusChangedEvent>,
        IMessageHandler<Place_AssignedEvent>
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
                              OwnerId = message.CreatorId,
                              Location = message.Location,
                              Title = message.Title,
                              Address = message.Address,
                              Description = message.Description,
                              CategoryId = message.CategoryId,
                              WorkDays = message.WorkDays
                          };
            _documentService.Save(doc);
        }

        public void Handle(Place_StatusChangedEvent message)
        {
            var query = Query.EQ("_id", message.PlaceId);
            var update = Update.Set("Status", message.Status);
            _documentService.Update(query,update);
        }

        public void Handle(Place_AssignedEvent message)
        {
            var query = Query.EQ("_id", message.PlaceId);
            var update = Update.Set("OwnerId", message.OwnerId);
            _documentService.Update(query, update);
        }
    }
}