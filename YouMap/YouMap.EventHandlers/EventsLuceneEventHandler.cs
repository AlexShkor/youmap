using System.Linq;
using Paralect.ServiceBus;
using YouMap.Documents.Lucene;
using YouMap.Domain.Data;
using YouMap.Domain.Events;

namespace YouMap.EventHandlers
{
    public class EventsLuceneEventHandler:
        IMessageHandler<User_EventAddedEvent>,
        IMessageHandler<User_EventMemberAddedEvent>
    {
        private readonly EventsLuceneService _eventsLuceneService;

        public EventsLuceneEventHandler(EventsLuceneService eventsLuceneService)
        {
            _eventsLuceneService = eventsLuceneService;
        }

        public void Handle(User_EventAddedEvent message)
        {
            var doc = new EventLucene()
                          {
                              Id = message.Id,
                              MembersIds = message.UsersIds.ToList(),
                          };
            _eventsLuceneService.Insert(doc);
        }

        public void Handle(User_EventMemberAddedEvent message)
        {
            var doc = _eventsLuceneService.GetById("_id",message.EventId);
            doc.MembersIds.Add(message.NewMemberId);
            _eventsLuceneService.Update(doc);
        }
    }
}