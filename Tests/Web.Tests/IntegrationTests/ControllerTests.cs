using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Web.Models;
using Xunit;

namespace Web.Tests.IntegrationTests
{
    [Collection("Integration Tests")]
    public class ControllerIntegrationTests : IDisposable //: IClassFixture<WebApplicationFactory<Products.Startup>>
    {
        private readonly HttpClient _client;
        private readonly TestServer _server;

        public ControllerIntegrationTests()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>());
            _client = _server.CreateClient();

            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: true)
             .AddEnvironmentVariables();

            IConfiguration config = builder.Build();
        }
        public void Dispose()
        {
            _client.Dispose();
            _server.Dispose();
        }

        [Fact]
        public async Task Index_ReturnsOkResult()
        {
            // Act
            var response = await _client.GetAsync("");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
        }

        [Fact]
        public async Task Search_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("Home/Search");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
        }

        [Fact]
        public async Task Add_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("Home/Add");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
        }

        [Fact]
        public async Task Search_WithParam_ReturnsOk()
        {
            // Act
            var content = new StringContent("searchstring=test", Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await _client.PostAsync("Home/Search", content);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
        }

        [Fact]
        public async Task Search_WithEmptyParam_ReturnsBadRequest()
        {
            // Act
            var content = new StringContent("", Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await _client.PostAsync("Home/Search", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Add_WithParam_ReturnsOk()
        {
            // Act
            var product = new Dictionary<string, string>
            {
                { "Name", "Test Product" },
                { "Count", "7" },
                { "Price", "13" }

            };
            var content = new FormUrlEncodedContent(product);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");


            var response = await _client.PostAsync("Home/Add", content);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
        }
        [Fact]
        public async Task Add_WithEmptyParam_ReturnsBadRequest()
        {
            // Act
            var product = new Dictionary<string, string>
            {
            };
            var content = new FormUrlEncodedContent(product);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");


            var response = await _client.PostAsync("Home/Add", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}

