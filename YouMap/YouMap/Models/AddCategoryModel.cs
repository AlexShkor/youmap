using System.ComponentModel.DataAnnotations;
using System.Web;

namespace YouMap.Models
{
    public class AddCategoryModel
    {
        [Display(Name = "���")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "������")]
        public HttpPostedFileBase Icon { get; set; }

        public string FileName { get; set; }

        [Display(Name = "���������� � ����?")]
        public bool IsTop { get; set; }

        public string Id { get; set; }
    }
}