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
                //mcsettings.SslSettings = new SslSettings { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
                var client = new MongoClient(settings.Value.ConnectionString);
                if (client != null)
                    _database = client.GetDatabase(settings.Value.Database);

                //var dbsnames = mongoClient.ListDatabaseNames().ToList();
                _products = _database.GetCollection<Product>("Products");
                //var collections = _database.ListCollectionNames().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Can not access to db server.", ex);
            }
        }
    }
}