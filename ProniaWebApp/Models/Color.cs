using System.ComponentModel.DataAnnotations;

namespace ProniaWebApp.Models
{
    public class Color:BaseEntity
    {
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }

    }
}
