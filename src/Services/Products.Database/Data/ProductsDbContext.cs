using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Model;
using MongoDB.Driver;
using Products.Database.Model;
using System;

namespace Data
{
    public class ProductsDbContext 
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Product> _products;
        public IMongoCollection<Product> Products => _products;

        public ProductsDbContext(IOptions<Settings> settings)
        {
            try
            {
                //MongoClientSettings mcsettings = MongoClientSettings.FromUrl(new MongoUrl(settings.Value.ConnectionString));
                //    mcsettings.SslSettings = new SslSettings { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
                var mongoClient = new MongoClient(settings.Value.ConnectionString);
                _database = mongoClient.GetDatabase(settings.Value.Database);
                _products = _database.GetCollection<Product>("Products");
            }
            catch (Exception ex)
            {
                throw new Exception("Can not access to db server.", ex);
            }
        }
    }
}