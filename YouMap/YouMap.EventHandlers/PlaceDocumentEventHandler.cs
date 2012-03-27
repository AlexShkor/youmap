using System.Linq;
using MongoDB.Driver.Builders;
using Paralect.ServiceBus;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Domain;
using YouMap.Domain.Data;
using YouMap.Domain.Enums;
using YouMap.Domain.Events;

namespace YouMap.EventHandlers
{
    public class PlaceDocumentEventHandler :
        IMessageHandler<Place_AddedEvent>,
        IMessageHandler<Place_StatusChangedEvent>,
        IMessageHandler<Place_UpdatedEvent>,
        IMessageHandler<Plave_LayerChangedEvent>,
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
                              Layer = message.Layer,
                              WorkDays = message.WorkDays.ToList(),
                              Logo = message.Logo,
                              Status = message.Status
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

        public void Handle(Place_UpdatedEvent message)
        {
            var doc = _documentService.GetById(message.Id);
            doc.OwnerId = message.CreatorId;
            doc.Location = message.Location;
            doc.Title = message.Title;
            doc.Layer = message.Layer;
            doc.Address = message.Address;
            doc.Description = message.Description;
            doc.CategoryId = message.CategoryId;
            doc.WorkDays = message.WorkDays.ToList();
            doc.Logo = message.Logo;
            _documentService.Save(doc);
        }

        public void Handle(Plave_LayerChangedEvent message)
        {
            var query = Query.EQ("_id", message.PlaceId);
            var update = Update.Set("Layer", message.Layer);
            _documentService.Update(query,update);
        }
    }
}