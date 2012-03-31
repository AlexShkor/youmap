using Paralect.ServiceBus;
using YouMap.Documents.Lucene;
using YouMap.Domain;
using YouMap.Domain.Events;

namespace YouMap.EventHandlers
{
    public class PlaceLuceneEventHandler:
        IMessageHandler<Place_UpdatedEvent>,
        IMessageHandler<Place_StatusChangedEvent>,
        IMessageHandler<Place_AddedEvent>
    {
        private readonly PlaceLuceneService _lucene;

        public PlaceLuceneEventHandler(PlaceLuceneService lucene)
        {
            _lucene = lucene;
        }

        public void Handle(Place_AddedEvent message)
        {
            var doc = new PlaceLucene
                          {
                              Id = message.Id,
                              Address = message.Address,
                              Memo = message.Description,
                              Title = message.Title,
                              Tags = message.Tags
                          };
            _lucene.Insert(doc);
        }

        public void Handle(Place_UpdatedEvent message)
        {
            var doc = new PlaceLucene
            {
                Id = message.Id,
                Address = message.Address,
                Memo = message.Description,
                Title = message.Title,
                Tags = message.Tags,
            };
            _lucene.Update(doc);
        }

        public void Handle(Place_StatusChangedEvent message)
        {
            var doc = _lucene.GetById("_id", message.PlaceId);
            doc.Status = message.Status;
            _lucene.Update(doc);
        }
    }
}