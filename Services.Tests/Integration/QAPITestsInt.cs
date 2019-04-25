using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

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
                .UseStartup<Products.Database.Startup>());
            _client = _server.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:8082");
        }
        public void Dispose()
        {
            _client.Dispose();
            _server.Dispose();
        }

        [Theory]
        [InlineData("/GetStat")]
        [InlineData("/GetList")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task CallGetStatReturnsProductStatDTO()
        {
            var response = await _client.GetAsync("/GetStat");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
