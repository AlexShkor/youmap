using System.Collections.Generic;
using System.Web.Mvc;

namespace YouMap.Models
{
    public class PlaceAssignModel
    {
        public string Title { get; set; }

        public string UserId { get; set; }

        public string Id { get; set; }

        public string Address { get; set; }

        public IEnumerable<SelectListItem> Users { get; set; }
    }
}