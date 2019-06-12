using AutoMapper;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
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
        private readonly IMapper _mapper;
        private readonly IDatabase db;

        public ProductRepository(IHttpClientFactory clientFactory, IConfiguration configuration, IMapper mapper, IConnectionMultiplexer connectionMultiplexor)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _mapper = mapper;
            db = connectionMultiplexor.GetDatabase();
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
                    var products = await response.Content.ReadAsAsync<IEnumerable<ProductDTO>>();
                    return _mapper.Map<IEnumerable<Product>>(products); 
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public async Task<int> GetCount()
        {
            return 0;
        }

        public async Task<decimal> GetSum()
        {
            return 0;
        }

        public async Task<int> GetTotal()
        {
            return 0;
        }

        public async Task<ProductsStatDTO> GetStat()
        {
            return null;
        }

        public Task UpdateStat(int count, int items, decimal price)
        {
            throw new NotImplementedException();
        }
    }
}
