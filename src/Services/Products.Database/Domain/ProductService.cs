using Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<bool> Add(ProductDTO product)
        {
            var res = await _productRepository.Add(product);
           return res;
        }

        public async Task<IEnumerable<ProductDTO>> GetList(string name)
        {
            var res = await _productRepository.GetList(name);
            return res; 
        }

        public async Task<ProductsStatDTO> GetStat()
        {
            var res = await _productRepository.GetStat();
            return res;
        }
    }
}