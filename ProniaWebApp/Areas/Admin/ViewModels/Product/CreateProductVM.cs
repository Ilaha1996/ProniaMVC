using ProniaWebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace ProniaWebApp.Areas.Admin.ViewModels
{
    public class CreateProductVM
    {
        public string Name { get; set; }
        public string Description { get; set; }

        [Required]
        public int? CategoryId { get; set; }

        [Required]
        public decimal? Price { get; set; }
        public string SKU { get; set; }

        public List<Category>? Categories { get; set; }
    }
}
