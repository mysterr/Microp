using Products.Database.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.Database.Infrastructure
{
    public interface IProductRepository
    {
        Task<ProductsStatDTO> GetStat();
        Task<IEnumerable<ProductDTO>> GetList(string name);
        Task<bool> Add(ProductDTO product);
    }

}
