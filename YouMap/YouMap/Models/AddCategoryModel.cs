using System.ComponentModel.DataAnnotations;
using System.Web;

namespace YouMap.Models
{
    public class AddCategoryModel
    {
        [Display(Name = "Имя")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Иконка")]
        public HttpPostedFileBase Icon { get; set; }

        [Display(Name = "Порядок")]
        public int Order { get; set; }

        public string FileName { get; set; }

        [Display(Name = "Отображать в меню?")]
        public bool IsTop { get; set; }

        public string Id { get; set; }
    }
}