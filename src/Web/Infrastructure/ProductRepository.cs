using Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Infrastructure
{
    public class ProductRepository : IRepository<Product>
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        List<Product> _products = new List<Product>();
        public ProductRepository(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
        }
        public async Task Create(Product item)
        {
            var productDTO = new ProductDTO
            {
                Name = item.Name,
                Count = item.Count,
                Price = item.Price
            };

            using (var qClient = _clientFactory.CreateClient("ProductsQueryClient"))
            {
                try
                {
                    var response = await qClient.PostAsJsonAsync("/api/Products", productDTO);
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }


        public async Task<IEnumerable<Product>> Get(string name)
        {
            using (var dClient = _clientFactory.CreateClient("ProductsDatabaseClient"))
            {
                try
                {
                    var response = await dClient.GetAsync($"/api/Products/GetList/{name}");
                    response.EnsureSuccessStatusCode();
                    var res = await response.Content.ReadAsAsync<IEnumerable<Product>>();
                    return res;
                }
                catch (Exception e)
                {
                    throw e;
                }
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
