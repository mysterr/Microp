using Domain.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Products.Database.Infrastructure;
using Products.Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Products.Database.Data
{
    public class ProductsDbContext : IProductsDbContext
    {
        private readonly IMongoDatabase _database;
        public IMongoCollection<Product> Products { get; }

        public ProductsDbContext(IOptions<Settings> settings)
        {
            try
            {
                //MongoClientSettings mcsettings = MongoClientSettings.FromUrl(new MongoUrl(settings.Value.ConnectionString));
                //mcsettings.SslSettings = new SslSettings { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
                var client = new MongoClient(settings.Value.ConnectionString);
                if (client != null)
                    _database = client.GetDatabase(settings.Value.Database);

                //var dbsnames = mongoClient.ListDatabaseNames().ToList();
                Products = _database.GetCollection<Product>("Products");
                //var collections = _database.ListCollectionNames().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot access to db server.", ex);
            }
        }

        public ProductsDbContext()
        {
        }

        public async Task<bool> AddAsync(Product product)
        {
            if (product.Price <= 0 | product.Count <= 0)
                return false;
            try
            {
                await Products.InsertOneAsync(product);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<ProductsStat> GetStatAsync()
        {
            try
            {
                var total = await Products.Aggregate().Group(p => p.Name,
                        g => new
                        {
                            itemsCount = g.Sum(f => f.Count),
                            totalSum = g.Sum(f => f.Price)
                        }).ToListAsync();

                return new ProductsStat
                {
                    ProductsCount = total.Count(),
                    ItemsCount = total.Sum(t => t.itemsCount),
                    Sum = total.Sum(t => t.totalSum)
                };
            }
            catch (Exception ex)
            {

                throw new DatabaseErrorException(ex);
            }
        }

        public async Task<IEnumerable<Product>> SearchAsync(string name)
        {
            var filter = Builders<Product>.Filter.Regex(f => f.Name, $"/{name ?? ""}/i");
            try
            {
                var products = await Products.Find(filter).ToListAsync();
                return products;
            }
            catch (Exception ex)
            {
                throw new DatabaseErrorException(ex);
            }
        }

        public async Task<IEnumerable<Product>> GetAsync(string name)
        {
            var filter = Builders<Product>.Filter.Eq(f => f.Name, name);
            try
            {
                var products = await Products.Find(filter).ToListAsync();
                return products;
            }
            catch (Exception ex)
            {
                throw new DatabaseErrorException(ex);
            }        }
    }
}