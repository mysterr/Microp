using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Products.Database;
using Products.Database.Data;

namespace Services.Tests
{
    [Collection("Integration Tests")]
    public class QAPIIntegrationTests :IDisposable //: IClassFixture<WebApplicationFactory<Products.Startup>>
    {
        private readonly HttpClient _client;
        private readonly TestServer _server;

        public QAPIIntegrationTests()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>());
            _client = _server.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:8082");
        }
        public void Dispose()
        {
            _client.Dispose();
            _server.Dispose();
        }

        [Theory]
        [InlineData("/api/Products/GetStat")]
        [InlineData("/api/Products/GetList?name=abc")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task CallGetStatReturnsOk()
        {
            var response = await _client.GetAsync("/api/Products/GetStat");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task CallGetStatReturnsProductStatDTO()
        {
            var response = await _client.GetAsync("/api/Products/GetStat");
            var result = await response.Content.ReadAsAsync<ProductsStatDTO>();
            Assert.True(result.ItemsCount > 0);
            Assert.True(result.ProductsCount > 0);
            Assert.True(result.Sum > 0);
        }
        [Fact]
        public async Task CallGetListReturnsProductDTOList()
        {
            var response = await _client.GetAsync("/api/Products/GetList?name=ab");
            var result = await response.Content.ReadAsAsync<IEnumerable<ProductDTO>>();
            Assert.NotEmpty(result);
            Assert.Contains("ab", result.First().Name);
        }
    }
}
