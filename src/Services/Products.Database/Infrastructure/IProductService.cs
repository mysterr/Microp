using Domain.Models;
using Products.Database.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.Database.Infrastructure
{
    public interface IProductService
    {
        Task<ProductsStat> GetStat();
        Task<IEnumerable<Product>> GetList(string name);
        Task<bool> Add(Product product);
    }
}