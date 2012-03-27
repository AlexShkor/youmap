using System;
using System.Collections.Generic;
using Paralect.Domain;
using YouMap.Domain.Data;
using YouMap.Framework;

namespace YouMap.Domain.Commands
{
    public class Place_UpdateCommand: Command
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public Location Location { get; set; }

        public string CategoryId { get; set; }

        public IEnumerable<DayOfWeek> WorkDays { get; set; }

        public string Logo { get; set; }

        public int Layer { get; set; }
    }

    public class  Place_UpdateCommandHandler: CommandHandler<Place_UpdateCommand>
    {
        public Place_UpdateCommandHandler(IRepository repository) : base(repository)
        {
        }

        public override void Handle(Place_UpdateCommand message)
        {
            var ar = Repository.GetById<PlaceAR>(message.Id);
            ar.SetCommandMetadata(message.Metadata);
             var data = new PlaceData
                           {
                               Id = message.Id,
                               Title = message.Title,
                               Address = message.Address,
                               Description = message.Description,
                               CreatorId = message.Metadata.UserId,
                               Location = message.Location,
                               WorkDays = message.WorkDays,
                               Layer = message.Layer,
                               Logo = message.Logo,
                               CategoryId = message.CategoryId
                           };
            ar.Update(data);
            Repository.Save(ar);
        }
    }
}