using Paralect.Domain;
using YouMap.Framework;

namespace YouMap.Domain.Commands
{
    public class Place_ChangeLayerCommand: Command
    {
        public string PlaceId { get; set; }

        public int Layer { get; set; }
    }

    public class Place_ChangeLayerCommandHandler: CommandHandler<Place_ChangeLayerCommand>
    {
        public override void Handle(Place_ChangeLayerCommand message)
        {
            var ar = Repository.GetById<PlaceAR>(message.PlaceId);
            ar.SetCommandMetadata(message.Metadata);
            ar.ChangeLayer(message.Layer);
            Repository.Save(ar);
        }
    }
}