using Paralect.Domain;

namespace YouMap.Domain.Events
{
    public class User_PasswordChangedEvent: Event
    {
        public string UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}