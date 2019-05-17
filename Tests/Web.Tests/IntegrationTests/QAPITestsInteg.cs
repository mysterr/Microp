using Domain.Models;
using EasyNetQ;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Web.Controllers;
using Web.Infrastructure;
using Xunit;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;

namespace Web.Tests.IntegrationTests
{
    public class QAPITestsInteg : IDisposable
    {
        private readonly HttpClient _client;
        private readonly TestServer _server;

        public QAPITestsInteg()
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
