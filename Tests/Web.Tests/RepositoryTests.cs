using AutoMapper;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Web.Infrastructure;
using Web.Models;
using Xunit;

namespace Web.Tests
{
    public class RepositoryTests
    {
        private readonly ProductRepository _productRepository;
        private readonly HttpClient _httpClient;
        private readonly Mock<IHttpClientFactory> _mockClientFactory;
        private readonly Mock<HttpMessageHandler> _handlerMock;
        private readonly Mock<IDatabase> _databaseMock;

        public RepositoryTests()
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: true)
                 .AddEnvironmentVariables();

            IConfiguration config = builder.Build();

            _handlerMock = new Mock<HttpMessageHandler>();
            HandlerMockSetup();

            _httpClient = new HttpClient(_handlerMock.Object)
            {
                BaseAddress = new Uri(config.GetSection("DatabaseService:ConnectionString").Value)
            };
            _mockClientFactory = new Mock<IHttpClientFactory>();
            _mockClientFactory.Setup(c => c.CreateClient("ProductsDatabaseClient")).Returns(_httpClient);
            _mockClientFactory.Setup(c => c.CreateClient("ProductsQueryClient")).Returns(_httpClient);
            var mapperMock = new Mock<IMapper>();
            _databaseMock = new Mock<IDatabase>();
            _databaseMock.Setup(d => d.HashGetAsync("products", It.IsAny<RedisValue>(), It.IsAny<CommandFlags>())).ReturnsAsync(100);
        
            var multiplexorMock = new Mock<IConnectionMultiplexer>();
            multiplexorMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_databaseMock.Object);

            _productRepository = new ProductRepository(_mockClientFactory.Object, mapperMock.Object, multiplexorMock.Object);
        }

        private void HandlerMockSetup()
        {

            var productList = new List<Product>
            {
                new Product
                {
                    Name = "Test",
                    Count = 5,
                    Price = 10M
                }
            };

            _handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri.AbsolutePath.StartsWith($"/api/Products/GetList")),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(JsonConvert.SerializeObject(productList), Encoding.Default, "application/json")
               })
               .Verifiable();

            _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
               "SendAsync",
               ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri.AbsolutePath.StartsWith($"/api/Products/GetStat")),
               ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new ProductsStatDTO { ItemsCount = 2, ProductsCount = 10, Sum = 15M }), Encoding.Default, "application/json")
            })
            .Verifiable();

            _handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post && r.RequestUri.AbsolutePath.StartsWith($"/api/Products")),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
               })
               .Verifiable();
        }

        [Fact]
        public async void Get_SendSearchQuery()
        {
            var result = await _productRepository.Get("abc");
            var res = Assert.IsAssignableFrom<IEnumerable<Product>>(result);

            _handlerMock.Protected().Verify(
               "SendAsync",
               Times.Once(),
               ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri.AbsolutePath.StartsWith($"/api/Products/GetList")),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async void Create_CallsAPI()
        {
            var product = new Product
            {
                Name = "Test",
                Count = 5,
                Price = 10M
            };
            await _productRepository.Create(product);

            _handlerMock.Protected().Verify(
               "SendAsync",
               Times.Once(),
               ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post && r.RequestUri.AbsolutePath.StartsWith($"/api/Products")),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async void GetCount_ReturnsInt()
        {
            var res = await _productRepository.GetCount();
            Assert.IsType<int>(res);
            Assert.NotEqual(0, res);
            _databaseMock.Verify(d => d.HashGetAsync("products", "count", It.IsAny<CommandFlags>()));
        }

        [Fact]
        public async void GetCount_IfEmpty_UpdateStat()
        {
            _databaseMock.Setup(d => d.HashGetAsync("products", It.IsAny<RedisValue>(), It.IsAny<CommandFlags>())).ReturnsAsync(new RedisValue());
            var res = await _productRepository.GetCount();
            Assert.IsType<int>(res);
            _handlerMock.Protected().Verify(
               "SendAsync",
               Times.Once(),
               ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri.AbsolutePath.StartsWith($"/api/Products/GetStat")),
               ItExpr.IsAny<CancellationToken>()
            );
            _databaseMock.Verify(d => d.HashSetAsync("products", "count", It.IsAny<RedisValue>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
        }

        [Fact]
        public async void GetSum_ReturnsDecimal()
        {
            var res = await _productRepository.GetSum();
            Assert.IsType<decimal>(res);
            Assert.NotEqual(0, res);
            _databaseMock.Verify(d => d.HashGetAsync("products", "sum", It.IsAny<CommandFlags>()));
        }
        [Fact]
        public async void GetSum_IfEmpty_UpdateStat()
        {
            _databaseMock.Setup(d => d.HashGetAsync("products", It.IsAny<RedisValue>(), It.IsAny<CommandFlags>())).ReturnsAsync(new RedisValue());
            var res = await _productRepository.GetSum();
            Assert.IsType<decimal>(res);
            _handlerMock.Protected().Verify(
               "SendAsync",
               Times.Once(),
               ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri.AbsolutePath.StartsWith($"/api/Products/GetStat")),
               ItExpr.IsAny<CancellationToken>()
            );
            _databaseMock.Verify(d => d.HashSetAsync("products", "sum", It.IsAny<RedisValue>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
        }
        [Fact]
        public async void GetTotal_ReturnsInt()
        {
            var res = await _productRepository.GetTotal();
            Assert.IsType<int>(res);
            Assert.NotEqual(0, res);
            _databaseMock.Verify(d => d.HashGetAsync("products", "items", It.IsAny<CommandFlags>()));
        }
        [Fact]
        public async void GetTotal_IfEmpty_UpdateStat()
        {
            _databaseMock.Setup(d => d.HashGetAsync("products", It.IsAny<RedisValue>(), It.IsAny<CommandFlags>())).ReturnsAsync(new RedisValue());
            var res = await _productRepository.GetTotal();
            Assert.IsType<int>(res);
            _handlerMock.Protected().Verify(
               "SendAsync",
               Times.Once(),
               ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri.AbsolutePath.StartsWith($"/api/Products/GetStat")),
               ItExpr.IsAny<CancellationToken>()
            );
            _databaseMock.Verify(d => d.HashSetAsync("products", "items", It.IsAny<RedisValue>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
        }

        [Fact]
        public async void UpdateStat_CallsIncrementDatabase()
        {
            await _productRepository.UpdateStat(1, 12, 33.6M);
            var sum = 100 + 12 * 33.6M;
            _databaseMock.Verify(d => d.HashIncrementAsync("products", "count", 1, It.IsAny<CommandFlags>()));
            _databaseMock.Verify(d => d.HashIncrementAsync("products", "items", 12, It.IsAny<CommandFlags>()));
            _databaseMock.Verify(d => d.HashSetAsync("products", "sum", sum.ToString(), It.IsAny<When>(), It.IsAny<CommandFlags>()));
        }
    }
}
