using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
