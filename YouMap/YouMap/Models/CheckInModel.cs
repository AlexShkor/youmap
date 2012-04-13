using System;
using YouMap.Controllers;
using YouMap.Domain.Data;

namespace YouMap.Models
{
    public class CheckInModel
    {
        public bool DisplayPlace { get; set; }

        public string Memo { get; set; }

        public string PlaceId { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string CheckInUrl { get; set; }

        public string LogoUrl { get; set; }

        public DateTime Visited { get; set; }

        public string RedirectUrl { get; set; }

        public bool Share { get; set; }

        public int LeftCount { get; set; }

        public bool Enabled
        {
            get
            {
                return Limited && LeftCount > 0;
            }
        }

        public bool Limited { get; set; }


        private const string CheckInMemoTemlate = @"Я сейчас в ""{0}"".";

        public void SetMemoWithTemplate(string title)
        {
            Memo = string.Format(CheckInMemoTemlate, title);
        }
    }
}