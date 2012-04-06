using System.Linq;
using Paralect.ServiceBus;
using YouMap.Documents.Lucene;
using YouMap.Documents.Services;
using YouMap.Domain.Data;
using YouMap.Domain.Events;

namespace YouMap.EventHandlers
{
    public class EventsLuceneEventHandler
        :
        IMessageHandler<User_EventAddedEvent>,
        IMessageHandler<User_EventMemberAddedEvent>
    {
        private readonly PlaceDocumentService _placeDocumentService;
        private readonly EventsLuceneService _eventsLuceneService;

        public EventsLuceneEventHandler(EventsLuceneService eventsLuceneService, PlaceDocumentService placeDocumentService)
        {
            _placeDocumentService = placeDocumentService;
            _eventsLuceneService = eventsLuceneService;
        }

        public void Handle(User_EventAddedEvent message)
        {
            ////Now we dont need to lucene search, we can you mongo collection
            return;
            var place = _placeDocumentService.GetById(message.PlaceId);
            var doc = new EventLucene()
                          {
                              Id = message.Id,
                              MembersIds = message.UsersIds.ToList(),
                              Memo = message.Memo,
                              Title = message.Title,
                              PlaceTitle = place.Title,
                              StartDate = message.Start
                          };
            _eventsLuceneService.Insert(doc);
        }

        public void Handle(User_EventMemberAddedEvent message)
        {
            ////Now we dont need to lucene search, we can you mongo collection
            return;
            var doc = _eventsLuceneService.GetById("_id",message.EventId);
            doc.MembersIds.Add(message.NewMemberId);
            _eventsLuceneService.Update(doc);
        }
    }
}