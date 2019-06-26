
using EasyNetQ;

namespace Domain.Models
{
    [Queue("product.q", ExchangeName = "product.ex")]
    public class ProductDTO
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public bool IsNew { get; set; }
    }
}
