using Paralect.Domain;
using Paralect.ServiceBus;

namespace YouMap.Framework
{
    public abstract class CommandHandler<T>: IMessageHandler<T>
    {
        protected IRepository Repository { get; set; }

        protected CommandHandler(IRepository repository)
        {
            Repository = repository;
        }

        public abstract void Handle(T message);
    }
}