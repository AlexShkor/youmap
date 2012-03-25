using Paralect.Domain;

namespace YouMap.Domain
{
    public class Category_UpdatedEvent : Event
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public bool IsTop { get; set; }

        public int Order { get; set; }
    }
}