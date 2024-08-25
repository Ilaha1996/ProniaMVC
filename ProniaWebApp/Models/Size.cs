using System.ComponentModel.DataAnnotations;

namespace ProniaWebApp.Models
{
    public class Size:BaseEntity
    {
        [Required]        
        public string Name { get; set; }
        public ICollection<ProductSize> ProductSizes { get; set; }

    }
}
