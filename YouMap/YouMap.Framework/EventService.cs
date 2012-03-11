using Paralect.Domain;
using Paralect.Domain.EventBus;

namespace YouMap.Framework
{
    public interface IEventService
    {
        void Send(params IEvent[] events);
    }

    public class EventService : IEventService
    {
        private readonly IEventBus _eventBus;

        public EventService(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Send(params IEvent[] events)
        {
            _eventBus.Publish(events);
        }
    }
}