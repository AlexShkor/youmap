using Paralect.Domain;
using Paralect.ServiceBus;
using YouMap.Domain.Data;
using YouMap.Framework;

namespace YouMap.Domain.Commands
{
    public class Place_UpdateLocationCommand: Command
    {
        public string Id { get; set; }

        public Location Location { get; set; }
    }

    public class Place_UpdateLocationCommandHandler: CommandHandler<Place_UpdateLocationCommand>
    {
        public override void Handle(Place_UpdateLocationCommand message)
        {
            var ar = Repository.GetById<PlaceAR>(message.Id);
            ar.SetCommandMetadata(message.Metadata);
            ar.UpdateLocation(message.Id, message.Location);
            Repository.Save(ar);
        }
    }
}