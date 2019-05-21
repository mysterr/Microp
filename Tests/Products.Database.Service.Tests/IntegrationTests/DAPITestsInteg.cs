using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Products.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Web.Tests.IntegrationTests
{
    [Collection("Integration Tests")]
    public class DAPIIntegrationTests : IDisposable //: IClassFixture<WebApplicationFactory<Products.Startup>>
    {
        private readonly HttpClient _client;
        private readonly TestServer _server;

        public DAPIIntegrationTests()
        {

            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: true)
             .AddEnvironmentVariables();

            IConfiguration config = builder.Build();

            _server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseConfiguration(config)
                .UseStartup<Startup>());
            _client = _server.CreateClient();

            _client.BaseAddress = new Uri(config.GetSection("DatabaseService:ConnectionString").Value);
        }
        public void Dispose()
        {
            _client.Dispose();
            _server.Dispose();
        }

        [Theory]
        [InlineData("/api/Products/GetStat")]
        [InlineData("/api/Products/GetList/abc")]
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
        public async Task CallGetStatReturnsProductStatDTO()
        {
            var response = await _client.GetAsync("/api/Products/GetStat");
            var reply = await response.Content.ReadAsStringAsync();
            Assert.NotNull(reply);
            var result = JsonConvert.DeserializeObject<ProductsStatDTO>(reply);
            Assert.True(result.ItemsCount > 0);
            Assert.True(result.ProductsCount > 0);
            Assert.True(result.Sum > 0);
        }
        [Fact]
        public async Task CallGetListReturnsProductDTOList()
        {
            var response = await _client.GetAsync("/api/Products/GetList/ab");
            var reply = await response.Content.ReadAsStringAsync();
            Assert.NotNull(reply);
            var result = JsonConvert.DeserializeObject<IEnumerable<ProductDTO>>(reply);
            Assert.NotEmpty(result);
            Assert.Contains("ab", result.First().Name);
        }
    }
}
