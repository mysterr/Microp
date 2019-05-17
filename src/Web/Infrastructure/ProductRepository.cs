using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Infrastructure
{
    public class ProductRepository : IRepository<Product>
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly HttpClient _client;

        List<Product> _products = new List<Product>();
        public ProductRepository(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _client = _clientFactory.CreateClient();
        }
        public async Task Create(Product item)
        {
            //var response = await _client.PostAsync(qServiceURL + "api/Products", JsonConvert.SerializeObject(item));

            await Task.Run(() => _products.Add(item));
        }

        public async Task<IEnumerable<Product>> Get(string name)
        {
            var res = await Task.Run(() => _products.Where(s => s.Name.Contains(name??"")));
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
