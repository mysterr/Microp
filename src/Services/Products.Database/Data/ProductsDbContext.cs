using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Products.Database.Model;
using System;

namespace Products.Database.Data
{
    public class ProductsDbContext
    {
        private readonly IMongoDatabase _database;
        virtual public IMongoCollection<Product> Products { get; }
        public ProductsDbContext()
        {

        }

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
    }
}