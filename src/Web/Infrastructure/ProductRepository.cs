using Domain.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Infrastructure
{
    public class ProductRepository : IRepository<Product>
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly string _DServiceBaseUrl;
        private readonly string _QServiceBaseUrl;

        List<Product> _products = new List<Product>();
        public ProductRepository(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _client = _clientFactory.CreateClient();
            _DServiceBaseUrl = configuration.GetSection("DatabaseService:ConnectionString").Value;
            _QServiceBaseUrl = configuration.GetSection("QueueService:ConnectionString").Value;
        }
        public async Task Create(Product item)
        {
            var productDTO = new ProductDTO
            {
                Name = item.Name,
                Count = item.Count,
                Price = item.Price
            };

            var content = new StringContent(JsonConvert.SerializeObject(productDTO), Encoding.UTF8, "application/json");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var uri = new Uri($"{_QServiceBaseUrl}/api/Products");

            await _client.PostAsync(uri, content);
        }

        public async Task<IEnumerable<Product>> Get(string name)
        {
            //var res = await Task.Run(() => _products.Where(s => s.Name.Contains(name??"")));
            var response = await _client.GetAsync($"{_DServiceBaseUrl}/api/Products/{name}");
            var content = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<IEnumerable<Product>>(content);
            return res;
        }

        public async Task<int> GetCount()
        {
            var res = await Task.Run(() => _products.Count());
            return res;
        }

        public async Task<decimal> GetSum()
        {
            var res = await Task.Run(() => _products.Sum(s => s.Price));
            return res;
        }

        public async Task<int> GetTotal()
        {
            var res = await Task.Run(() => _products.Sum(s => s.Count));
            return res;
        }
    }
}
