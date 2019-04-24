using Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    class ProductRepository : IProductRepository
    {
        public Task<ProductsStatDTO> GetStat()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductDTO>> GetList(string name)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Add(ProductDTO product)
        {
            throw new NotImplementedException();
        }
    }
}
