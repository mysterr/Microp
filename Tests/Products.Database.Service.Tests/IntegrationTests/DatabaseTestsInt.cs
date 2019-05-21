using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Products.Database.Model;
using Products.Database.Infrastructure;
using Products.Database.Data;
using System.IO;
using Domain.Models;
using AutoMapper;

namespace QDatabaseTests
{
    public class MyOptions : IOptions<Settings>
    {
        private readonly string ConnectionString;
        private readonly string Database;
        public Settings Value => new Settings { ConnectionString = ConnectionString, Database = Database };

        public MyOptions()
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: true)
                 .AddEnvironmentVariables();

            IConfiguration config = builder.Build();
            ConnectionString = config.GetSection("MongoConnection:ConnectionString").Value;
            Database = config.GetSection("MongoConnection:Database").Value;
        }
    }

    [Collection("Integration Tests")]
    public class MongoRepositoryTest
    {
        private readonly IProductRepository _productRepository;

        public MongoRepositoryTest()
        {
            //var option = new Mock<IOptions<Settings>>();
            //var myOptions = new MyOptions();
            //option.Setup(o => o.Value).Returns(myOptions.Value);
            var productList = new List<Product>
            {
                new Product { Id = new Guid(), Name = "abcde", Count = 2, Price = 13M },
                new Product { Id = new Guid(), Name = "hello", Count = 8, Price = 2.5M }
            };

            var context = new ProductsDbContext();
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DomainProfile());
            });
            var mapper = mockMapper.CreateMapper();
            _productRepository = new ProductRepository(context, mapper);
        }

        [Fact]
        public async Task GetStatReturnsProductsStatDTOAsync()
        {
            var result = await _productRepository.GetStat();

            Assert.IsType<ProductsStatDTO>(result);
        }

        [Fact]
        public async Task GetStatReturnNotNullAndNotZero()
        {
            var res = await _productRepository.GetStat();

            Assert.NotNull(res);
            Assert.NotEqual(0, res.ItemsCount);
            Assert.NotEqual(0, res.ProductsCount);
            Assert.NotEqual(0, res.Sum);
        }
        [Fact]
        public async Task GetListReturnsListOfProductDTOAsync()
        {
            var list = await _productRepository.GetList("abc");

            Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(list);
        }
        [Fact]
        public async Task CanAddProduct()
        {
            await _productRepository.Add(new ProductDTO());
        }
    }
}
