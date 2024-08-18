using System.ComponentModel.DataAnnotations;

namespace ProniaWebApp.Models
{
    public class Size:BaseEntity
    {
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

    }
}
