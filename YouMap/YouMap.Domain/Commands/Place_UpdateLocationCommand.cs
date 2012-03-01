using Paralect.Domain;
using Paralect.ServiceBus;
using YouMap.Domain.Data;
using mPower.Framework;

namespace YouMap.Domain.Commands
{
    public class Place_UpdateLocationCommand: Command
    {
        public string Id { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }
    }

    public class Place_UpdateLocationCommandHandler: CommandHandler<Place_UpdateLocationCommand>
    {
        public Place_UpdateLocationCommandHandler(IRepository repository)
            : base(repository)
        {
        }

        public override void Handle(Place_UpdateLocationCommand message)
        {
            var ar = Repository.GetById<PlaceAR>(message.Id);
            ar.SetCommandMetadata(message.Metadata);
            ar.UpdateLocation(message.Id, new Location(message.Latitude, message.Longitude));
            Repository.Save(ar);
        }
    }
}