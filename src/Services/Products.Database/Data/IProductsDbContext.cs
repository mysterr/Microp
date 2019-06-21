using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;
using MongoDB.Driver;
using Products.Database.Model;

namespace Products.Database.Data
{
    public interface IProductsDbContext
    {
        IMongoCollection<Product> Products { get; }

        Task<bool> AddAsync(Product product);
        Task<IEnumerable<Product>> GetAsync(string name);
        Task<ProductsStat> GetStatAsync();
    }
}