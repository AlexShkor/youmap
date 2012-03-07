using System.Web.Mvc;

namespace YouMap.Models
{
    public class UserViewModel
    {
        public bool IsAuthenticated { get; set; }

        public bool DisplayAdmin { get; set; }

        public string DisplayName { get; set; }

        public LogOnModel LogOnModel { get; set; }

        public UserViewModel()
        {
            LogOnModel = new LogOnModel();
        }
    }
}