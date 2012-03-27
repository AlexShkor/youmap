using Paralect.Domain;

namespace YouMap.Domain.Events
{
    public class Plave_LayerChangedEvent: Event
    {
        public string PlaceId { get; set; }

        public int Layer { get; set; }
    }
}