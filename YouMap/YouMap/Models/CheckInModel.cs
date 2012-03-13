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


        private const string CheckInMemoTemlate = "Я сейчас в {0}.";

        public void SetMemoWithTemplate(string title)
        {
            Memo = string.Format(CheckInMemoTemlate, title);
        }
    }
}