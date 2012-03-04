using YouMap.Domain.Data;

namespace YouMap.Models
{
    public class PlaceSearchModel
    {
        public string Serach { get; set; }

        public Location UserLocation { get; set; }
    }
}