using System;
using System.Collections.Generic;
using Paralect.Domain;
using Paralect.ServiceBus;
using YouMap.Domain.Data;
using mPower.Framework;

namespace YouMap.Domain.Commands
{
    public class Place_CreateCommand: Command
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public Location Location { get; set; }

        public string CategoryId { get; set; }

        public IEnumerable<DayOfWeek> WorkDays { get; set; }
    }

    public class Place_CreateCommandHandler : CommandHandler<Place_CreateCommand>
    {
        public Place_CreateCommandHandler(IRepository repository) : base(repository)
        {
        }

        public override void Handle(Place_CreateCommand message)
        {
            var data = new PlaceData
                           {
                               Id = message.Id,
                               Title = message.Title,
                               Address = message.Address,
                               Description = message.Description,
                               CreatorId = message.Metadata.UserId,
                               Location = message.Location,
                               WorkDays = message.WorkDays,
                               CategoryId = message.CategoryId
                           };
            var ar = new PlaceAR(data,message.Metadata);
            Repository.Save(ar);
            
        }
    }
}