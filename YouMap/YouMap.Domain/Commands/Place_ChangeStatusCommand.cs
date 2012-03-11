using Paralect.Domain;
using YouMap.Domain.Enums;
using YouMap.Framework;

namespace YouMap.Domain.Commands
{
    public class Place_ChangeStatusCommand: Command
    {
        public PlaceStatusEnum Status { get; set; }

        public string PlaceId { get; set; }
    }

    public class Place_ChangeStatusCommandHandler: CommandHandler<Place_ChangeStatusCommand>
    {
        public Place_ChangeStatusCommandHandler(IRepository repository) : base(repository)
        {
        }

        public override void Handle(Place_ChangeStatusCommand message)
        {
            var ar = Repository.GetById<PlaceAR>(message.PlaceId);
            ar.SetCommandMetadata(message.Metadata);
            ar.ChangeStatus(message.Status);
            Repository.Save(ar);
        }
    }
}