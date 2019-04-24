using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Data;

namespace Infrastructure
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            this._productRepository = productRepository;
        }

        public async Task<bool> Add(ProductDTO product)
        {
           return false;
        }

        public async Task<IEnumerable<ProductDTO>> GetList(string name)
        {
            var list = new List<ProductDTO>();
            list.Add(new ProductDTO { Count = 1, Name = "aaabbb", Price = 10.1M });
            list.Add(new ProductDTO { Count = 3, Name = "zzz", Price = 1.9M });
            return list.Where(s => s.Name.Contains(name));
        }

        public async Task<ProductsStatDTO> GetStat()
        {
            return new ProductsStatDTO { ItemsCount = 3, ProductsCount = 2, Sum = 12M };
        }
    }
}