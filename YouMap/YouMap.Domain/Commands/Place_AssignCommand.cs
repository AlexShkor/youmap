using Paralect.Domain;
using YouMap.Framework;

namespace YouMap.Domain.Commands
{
    public class Place_AssignCommand: Command
    {
        public string PlaceId { get; set; }
        public string OwnerId { get; set; }
    }

    public class Place_AssignCommandHandler: CommandHandler<Place_AssignCommand>
    {
        public Place_AssignCommandHandler(IRepository repository) : base(repository)
        {
        }

        public override void Handle(Place_AssignCommand message)
        {
            var ar = Repository.GetById<PlaceAR>(message.PlaceId);
            ar.SetCommandMetadata(message.Metadata);
            ar.AssignToUser(message.OwnerId);
            Repository.Save(ar);
        }
    }
}