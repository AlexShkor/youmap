using Paralect.ServiceBus;
using YouMap.Documents.Lucene;
using YouMap.Domain;

namespace YouMap.EventHandlers
{
    public class PlaceLuceneEventHandler:IMessageHandler<Place_AddedEvent>
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
                              Title = message.Title
                          };
            _lucene.Insert(doc);
        }
    }
}