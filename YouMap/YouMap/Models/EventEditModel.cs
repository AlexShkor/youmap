using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YouMap.Models
{
    public class EventEditModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "¬ведите название")]
        public string Title { get; set; }
        public string Memo { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        [Required(ErrorMessage = "¬ведите врем€ и дату начала")]
        public DateTime? Start { get; set; }
        public List<string> UsersIds { get; set; }
        public bool Private { get; set; }
        public string PlaceId { get; set; }
        public string PlateTitle { get; set; }

        public EventEditModel()
        {
            UsersIds = new List<string>();
        }
    }
}