using System.ComponentModel.DataAnnotations;

namespace ProniaWebApp.Models
{
    public class Tag:BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

    }
}
