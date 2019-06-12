using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Web.Tests.IntegrationTests
{

    [Collection("Integration Tests")]
    [Trait("Category", "Integration")]
    public class ControllerIntegrationTests : IDisposable//, IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly TestServer _server;
        private readonly IConfiguration config;
        //private readonly WebApplicationFactory<Startup> _factory;

        public ITestOutputHelper Output { get; }

        public ControllerIntegrationTests(ITestOutputHelper output)//,  WebApplicationFactory<Startup> factory)
        {
            Output = output;
            //_factory = factory;
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: true)
             .AddEnvironmentVariables();

            config = builder.Build();

            _server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>()
                .UseConfiguration(config));
            _client = _server.CreateClient();


            //_client = _factory.WithWebHostBuilder(b =>
            //    {
            //        b.ConfigureAppConfiguration(c => c.AddJsonFile(Directory.GetCurrentDirectory() + @"\appsettings.test.json"));
            //    })
            //    .CreateClient(
            //    new WebApplicationFactoryClientOptions
            //    {
            //        BaseAddress = new Uri(config.GetSection("Web:ConnectionString").Value),
            //        AllowAutoRedirect = false
            //    });
            //_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


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
            var content = new StringContent("searchstring=test", Encoding.Default, "application/x-www-form-urlencoded");

            var response = await _client.PostAsync("Home/Search", content);
            var reply = response.Content.ReadAsStringAsync().Result;
            Output.WriteLine(reply);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
        }

        [Fact]
        public async Task Search_WithEmptyParam_ReturnsBadRequest()
        {
            // Act
            var content = new StringContent("", Encoding.Default, "application/x-www-form-urlencoded");

            var response = await _client.PostAsync("Home/Search", content);
            var reply = response.Content.ReadAsStringAsync().Result;
            Output.WriteLine(reply);

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

