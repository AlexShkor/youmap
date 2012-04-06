using Paralect.ServiceBus;
using YouMap.Domain.Events;

namespace YouMap.EventHandlers
{
    public class EventDocumentEventHandler:
                IMessageHandler<User_EventAddedEvent>,
        IMessageHandler<User_EventMemberAddedEvent>
    {
        public void Handle(User_EventAddedEvent message)
        {
        }

        public void Handle(User_EventMemberAddedEvent message)
        {
        }
    }
}