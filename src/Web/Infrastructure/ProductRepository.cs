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
        private readonly IMapper _mapper;
        private readonly IDatabase db;

        public ProductRepository(IHttpClientFactory clientFactory, IMapper mapper, IConnectionMultiplexer connectionMultiplexor)
        {
            _clientFactory = clientFactory;
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
                    using (var response = await qClient.PostAsJsonAsync("/api/Products", productDTO))
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
                    using (var response = await dClient.GetAsync($"/api/Products/GetList/{name}"))
                    {
                        response.EnsureSuccessStatusCode();
                        var products = await response.Content.ReadAsAsync<IEnumerable<ProductDTO>>();
                        return _mapper.Map<IEnumerable<Product>>(products);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public async Task<int> GetCount()
        {
            var count = await db.HashGetAsync("products", "count");
            if (count.IsNull)
            {
                var productsCount = (await GetStat()).ProductsCount;
                await db.HashSetAsync("products", "count", productsCount);
                await db.KeyExpireAsync("products", DateTime.Now.AddDays(1));
                return productsCount;
            }
            if (count.TryParse(out int val))
                return val;
            else
                return 0;
        }

        public async Task<decimal> GetSum()
        {
            var sum = await db.HashGetAsync("products", "sum");
            if (sum.IsNull)
            {
                var productsSum = (await GetStat()).Sum;
                await db.HashSetAsync("products", "sum", productsSum.ToString());
                await db.KeyExpireAsync("products", DateTime.Now.AddDays(1));
                return productsSum;
            }
            if (decimal.TryParse(sum, out decimal val))
                return val;
            else
                return 0;
        }

        public async Task<int> GetTotal()
        {
            var total = await db.HashGetAsync("products", "items");
            if (total.IsNull)
            {
                var productsItemsCount = (await GetStat()).ItemsCount;
                await db.HashSetAsync("products", "items", productsItemsCount);
                await db.KeyExpireAsync("products", DateTime.Now.AddDays(1));
                return productsItemsCount;
            }
            if (total.TryParse(out int val))
                return val;
            else
                return 0;
        }

        public async Task UpdateStat(int count, int items, decimal price)
        {
            await db.HashIncrementAsync("products", "count", count);
            await db.HashIncrementAsync("products", "items", items);
            price = price * items + await GetSum();
            await db.HashSetAsync("products", "sum", price.ToString());

        }

        private async Task<ProductsStatDTO> GetStat()
        {
            using (var dClient = _clientFactory.CreateClient("ProductsDatabaseClient"))
            {
                try
                {
                    using (var response = await dClient.GetAsync($"/api/Products/GetStat"))
                    {
                        response.EnsureSuccessStatusCode();
                        var products = await response.Content.ReadAsAsync<ProductsStatDTO>();
                        return products;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }


    }
}
