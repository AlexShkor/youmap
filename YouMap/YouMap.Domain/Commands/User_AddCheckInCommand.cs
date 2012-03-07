using Paralect.Domain;
using YouMap.Domain.Data;
using mPower.Framework;

namespace YouMap.Domain.Commands
{
    public class User_AddCheckInCommand: Command
    {
        public string Memo { get; set; }

        public string Title { get; set; }

        public Location Location { get; set; }

        public string PlaceId { get; set; }

        public string UserId { get; set; }
    }
    public class User_AddCheckInCommandHandler: CommandHandler<User_AddCheckInCommand>
    {
        public User_AddCheckInCommandHandler(IRepository repository) : base(repository)
        {
        }

        public override void Handle(User_AddCheckInCommand message)
        {
            var ar = Repository.GetById<UserAR>(message.UserId);
            ar.SetCommandMetadata(message.Metadata);
            var data = new CheckInData
                           {
                               Memo = message.Memo,
                               Title = message.Title,
                               Location = message.Location,
                               PlaceId = message.PlaceId
                           };
            ar.AddCheckIn(data);
            Repository.Save(ar);
        }
    }
}