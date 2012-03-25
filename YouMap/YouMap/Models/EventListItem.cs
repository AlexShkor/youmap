using System.Collections.Generic;
using System.Web.Script.Serialization;

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

        public string ActionUrl { get; set; }

        public string ActionTitle { get; set; }

        public string LinkClass { get; set; }

        public bool Private { get; set; }

        public string ShareUrl { get; set; }

        public string ToJson()
        {
            var js = new JavaScriptSerializer();
            return js.Serialize(this);
        }
    }
}