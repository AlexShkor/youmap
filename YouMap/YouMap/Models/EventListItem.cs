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

        public string OwnerId { get; set; }

        public string OwnerVkId { get; set; }

        public string OwnerName { get; set; }

        public string PlaceId { get; set; }

        public string PlaceTitle { get; set; }

        public object Memo { get; set; }

        public bool Started { get; set; }

        public bool DisplayUsers { get; set; }
    }
}