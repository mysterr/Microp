using Domain.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Infrastructure
{
    public class ProductRepository : IRepository<Product>
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly HttpClient _dClient;
        private readonly HttpClient _qClient;
        private readonly IConfiguration _configuration;
        private readonly string _DServiceBaseUrl;
        private readonly string _QServiceBaseUrl;

        List<Product> _products = new List<Product>();
        public ProductRepository(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _dClient = _clientFactory.CreateClient("ProductsClient");
            _qClient = _clientFactory.CreateClient("ProductsClient");
            _DServiceBaseUrl = configuration.GetSection("DatabaseService:ConnectionString").Value;
            _QServiceBaseUrl = configuration.GetSection("QueueService:ConnectionString").Value;
            _dClient.BaseAddress = new Uri(_DServiceBaseUrl);
            _qClient.BaseAddress = new Uri(_QServiceBaseUrl);
        }
        public async Task Create(Product item)
        {
            var productDTO = new ProductDTO
            {
                Name = item.Name,
                Count = item.Count,
                Price = item.Price
            };

            await _qClient.PostAsJsonAsync("/api/Products", productDTO);
        }

        public async Task<IEnumerable<Product>> Get(string name)
        {            
            try
            {
                var response = await _dClient.GetAsync($"/api/Products/{name}");
                var res = await response.Content.ReadAsAsync<IEnumerable<Product>>();
                return res;
            }
            catch (Exception e)
            {
                throw e;
            }
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
