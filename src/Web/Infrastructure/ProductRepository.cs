using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Infrastructure
{
    public class ProductRepository : IRepository<Product>
    {
        internal List<Product> _products = new List<Product>();
        public async Task Create(Product item)
        {
            await Task.Run(() => _products.Add(item));
        }

        public async Task<IEnumerable<Product>> Get(string name)
        {
            var res = await Task.Run(() => _products.Where(s => s.Name.Contains(name)));
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
