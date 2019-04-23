using Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    class Repository : IProductRepository
    {
        public Task<ProductsStatDTO> GetStat()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductDTO>> GetList(string name)
        {
            throw new NotImplementedException();
        }
    }
}
