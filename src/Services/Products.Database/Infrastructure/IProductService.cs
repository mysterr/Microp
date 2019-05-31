using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.Database.Infrastructure
{
    public interface IProductService
    {
        Task<ProductsStatDTO> GetStat();
        Task<IEnumerable<ProductDTO>> GetList(string name);
        Task<bool> Add(ProductDTO product);
    }
}