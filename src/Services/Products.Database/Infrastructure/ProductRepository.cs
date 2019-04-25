using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class ProductRepository : IProductRepository
    {
        private readonly List<ProductDTO> list = new List<ProductDTO>();
        public ProductRepository()
        {
            list.Add(new ProductDTO { Count = 1, Name = "aaabbb", Price = 10.1M });
            list.Add(new ProductDTO { Count = 3, Name = "zzz", Price = 1.9M });
        }
        public Task<ProductsStatDTO> GetStat()
        {
            return Task.Run(() => new ProductsStatDTO { ItemsCount = list.Sum(s => s.Count), ProductsCount = list.Count(), Sum = list.Sum(s => s.Price) });
        }

        public Task<IEnumerable<ProductDTO>> GetList(string name)
        {
            return Task.Run(() => list.Where(s => s.Name.Contains(name)));
        }

        public Task<bool> Add(ProductDTO product)
        {
            list.Add(product);
            return Task.Run(() => true);
        }
    }
}
