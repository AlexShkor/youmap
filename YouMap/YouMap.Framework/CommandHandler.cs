using Paralect.Domain;
using Paralect.ServiceBus;
using StructureMap.Attributes;

namespace YouMap.Framework
{
    public abstract class CommandHandler<T>: IMessageHandler<T>
    {
        [SetterProperty]
        public IRepository Repository { get; set; }

        public abstract void Handle(T message);
    }
}