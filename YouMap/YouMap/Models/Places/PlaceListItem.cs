using System.Collections.Generic;

namespace YouMap.Models
{
    public class PlaceListItem
    {
        public string Title { get; set; }

        public string Address { get; set; }

        public string Description { get; set; }

        public string Icon { get; set; }

        public string Id { get; set; }

        public string HideLabel { get; set; }

        public string HideAction { get; set; }

        public bool DisplayBlockAction { get; set; }

        public int Layer { get; set; }

        public List<string> Tags { get; set; }

        public string MapUrl { get; set; }

        public string Distance { get; set; }

        public PlaceListItem()
        {
            Tags = new List<string>();
        }
    }
}