using System.Collections.Generic;

namespace YouMap.Models
{
    public class EventListItem
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string StartDate { get; set; }

        public string Url { get; set; }

        public List<string> UsersIds { get; set; }
    }
}