using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Product
    {
        [Required(ErrorMessage = "Enter product name")]
        public string Name { get; set; }
        [Display(Name = "Quantity")]
        public int Count { get; set; }
        public decimal Price { get; set; }

        public Product()
        {
        }

        public Product(string name, int count, decimal price)
        {
            this.Name = name;
            this.Count = count;
            this.Price = price;
        }
    }
}
