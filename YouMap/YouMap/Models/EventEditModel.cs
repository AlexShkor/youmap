using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using YouMap.Domain.Data;

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
        public string Hour { get; set; }
        public string Minute { get; set; }
        public IEnumerable<SelectListItem> AvailableHours { get; set; }
        public IEnumerable<SelectListItem> AvailableMinutes { get; set; } 
        public List<string> UserIds { get; set; }
        public List<string> UserNames { get; set; }
        public bool Private { get; set; }
        public string PlaceId { get; set; }
        public string PlaceTitle { get; set; }

        public EventEditModel()
        {
            UserIds = new List<string>();
            UserNames = new List<string>();
            Start = DateTime.Now.AddHours(2);
            Hour = Start.Value.Hour.ToString(CultureInfo.InvariantCulture);
            Minute = "00";
            AvailableHours = new SelectList(Enumerable.Range(0,24));
            AvailableMinutes = new SelectList(Enumerable.Range(0,12).Select(x=> (x*5).ToString("00")));
        }

        public DateTime GetStartDateTime()
        {
            return Start.Value.AddHours(int.Parse(Hour)).AddMinutes(int.Parse(Minute));
        }

        public IEnumerable<Friend> GetMembers()
        {
            return UserIds.Zip(UserNames, (id, name) => new Friend()
                                                            {
                                                                VkId = id,
                                                                FullName = name
                                                            });
        }
    }
}