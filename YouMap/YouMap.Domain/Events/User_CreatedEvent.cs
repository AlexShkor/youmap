using Paralect.Domain;

namespace YouMap.Domain.Events
{
    public class User_CreatedEvent: Event
    {
        public string Password { get; set; }

        public string Email { get; set; }

        public string UserId { get; set; }
    }
}