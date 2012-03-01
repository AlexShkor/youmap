using Paralect.Domain;

namespace YouMap.Domain.Events
{
    public class Category_CreatedEvent : Event
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool IsTop { get; set; }
    }
}