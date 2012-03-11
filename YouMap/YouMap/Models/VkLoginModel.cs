using System.ComponentModel.DataAnnotations;

namespace YouMap.Models
{
    public class VkLoginModel
    {
        public long Expire { get; set; }

        [Required]
        public string Sig { get; set; }

        [Required]
        public string Sid { get; set; }

        [Required]
        public string Mid { get; set; }

        public string Secret { get; set; }


        public String Domain { get; set; }

        [Required]
        public String FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Id { get; set; }

        public String Href { get; set; }

        public String Nickname { get; set; }
    }
}