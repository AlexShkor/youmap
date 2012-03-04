using System.ComponentModel.DataAnnotations;
using System.Web;

namespace YouMap.Controllers
{
    public class AddCategoryModel
    {
        [Display(Name = "»м€")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "»конка")]
        public HttpPostedFileBase Icon { get; set; }

        public string FileName { get; set; }

        [Display(Name = "ќтображать в меню?")]
        public bool IsTop { get; set; }

        public string Id { get; set; }
    }
}