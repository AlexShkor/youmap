using Paralect.Domain;

namespace YouMap.Domain.Commands
{
    public class User_ChangePasswordCommand: Command
    {
        public string UserId { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
    }
}