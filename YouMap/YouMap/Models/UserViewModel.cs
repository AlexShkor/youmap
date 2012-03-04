namespace YouMap.Models
{
    public class UserViewModel
    {
        public bool IsAuthenticated { get; set; }

        public bool DisplayAdmin { get; set; }

        public string DisplayName { get; set; }
    }
}