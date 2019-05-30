using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public RepositoryTests()
        {
            var product = new Product
            {
                Name = "Test",
                Count = 5,
                Price = 10M
            };

            var productList = new List<Product>
            {
                product
            };

            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: true)
                 .AddEnvironmentVariables();

            IConfiguration config = builder.Build();

            _handlerMock = new Mock<HttpMessageHandler>();
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
                  ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post && r.RequestUri.AbsolutePath.StartsWith($"/api/Products")),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
               })
               .Verifiable();

            // use real http client with mocked handler here
            _httpClient = new HttpClient(_handlerMock.Object);
            _mockClientFactory = new Mock<IHttpClientFactory>();
            _mockClientFactory.Setup(c => c.CreateClient("ProductsClient")).Returns(_httpClient);
            _productRepository = new ProductRepository(_mockClientFactory.Object, config);
        }

        [Fact]
        public async void Get_SendSearchQuery()
        {
            var result = await _productRepository.Get("abc");
            var res = Assert.IsAssignableFrom<IEnumerable<Product>>(result);
            Assert.NotEmpty(res);

            _mockClientFactory.VerifyAll();
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
            _mockClientFactory.VerifyAll();
            _handlerMock.Protected().Verify(
               "SendAsync",
               Times.Once(),
               ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post && r.RequestUri.AbsolutePath.StartsWith($"/api/Products")),
               ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
