using System;
using System.Collections.Generic;

namespace YouMap.Domain.Data
{
    public interface IEventData
    {
        string Title { get; set; }
        string Memo { get; set; }
        Location Location { get; set; }
        DateTime Start { get; set; }
        IEnumerable<string> UsersIds { get; set; }
        bool Private { get; set; }
        string PlaceId { get; set; }
        string OwnerId { get; set; }
    }
}