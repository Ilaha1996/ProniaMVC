namespace ProniaWebApp.Models
{
    public class Category:BaseEntity
    {
        public string Name { get; set; }

        //relational
        public ICollection<Product> Products { get; set; }
    }
}
