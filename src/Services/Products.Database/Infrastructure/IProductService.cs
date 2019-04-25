using Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IProductService
    {
        Task<ProductsStatDTO> GetStat();
        Task<IEnumerable<ProductDTO>> GetList(string name);
        Task<bool> Add(ProductDTO product);
    }
}