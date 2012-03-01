using Paralect.Domain;

namespace YouMap.Domain
{
    public class Category_DeletedEvent : Event
    {
        public string Id { get; set; }
    }
}