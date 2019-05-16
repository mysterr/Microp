using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Products.Queue.Controllers;
using Products.Queue.Data;
using Products.Queue.Infrastructure;
using System.IO;
using Xunit;

namespace Product.Queue.Service.Test.IntegrationTests
{
    public class APITestsInteg
    {
        private readonly ProductsController _productController;
        private readonly ProductRepository _productRepository;
        private readonly IBus _bus;

        public APITestsInteg()
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: true)
             .AddEnvironmentVariables();

            IConfiguration config = builder.Build();
            _bus = RabbitHutch.CreateBus(config.GetSection("RabbitConnection:ConnectionString").Value);
            _productRepository = new ProductRepository(_bus);
            _productController = new ProductsController(_productRepository);
        }

        [Fact]
        public async void CanAddProduct()
        {
            var productDTO = new ProductDTO
            {
                Name = "Test",
                Count = 3,
                Price = 9M
            };
            await _productController.Add(productDTO);
        }
    }
}
