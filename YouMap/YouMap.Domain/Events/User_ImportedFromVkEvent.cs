using Paralect.Domain;
using YouMap.Domain.Data;

namespace YouMap.Domain
{
    public class User_ImportedFromVkEvent : Event
    {
        public string UserId { get; set; }
        public VkData Vk { get; set; }
    }
}