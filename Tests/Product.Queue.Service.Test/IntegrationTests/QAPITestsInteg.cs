using Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using Products.Queue;

namespace IntegrationTests
{
    public class QAPIIntegrationTests : IDisposable
    {
        private readonly HttpClient _client;
        private readonly TestServer _server;

        public QAPIIntegrationTests()
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

        }
        public void Dispose()
        {
            _client.Dispose();
            _server.Dispose();
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
            HttpContent content = new StringContent(JsonConvert.SerializeObject(productDTO));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await _client.PostAsync("/api/Products", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
