using AutoMapper;
using Domain.Models;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Products.Database.Data;
using Products.Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Products.Database.Infrastructure
{
    public class ProductRepository : IProductRepository
    {
        private readonly IProductsDbContext _context;
        private readonly IBus _bus;
        private readonly IMapper _mapper;

        public ProductRepository(IProductsDbContext context, IBus bus, IMapper mapper)
        {
            _context = context;
            _bus = bus;
            _mapper = mapper;
        }
        public async Task<ProductsStat> GetStat()
        {
            try
            {
                var productsStat = await _context.GetStatAsync();
                if (productsStat == null)
                    productsStat = new ProductsStat
                    {
                        ProductsCount = 0,
                        ItemsCount = 0,
                        Sum = 0
                    };
                return productsStat;
            }
            catch (Exception ex)
            {

                throw new DatabaseErrorException(ex);
            }
        }

        public async Task<IEnumerable<Product>> GetList(string name)
        {
            try
            {
                var products = await _context.GetAsync(name);
                if (products.Any())
                    return products; 
                else
                    return new List<Product>();
            }
            catch (Exception ex)
            {
                throw new DatabaseErrorException(ex);
            }
        }

        public async Task<bool> Add(Product product)
        {
            try
            {
                if (product.Price <= 0 | product.Count <= 0)
                    return false;
                var res = await _context.AddAsync(product);
                var productDto = _mapper.Map<ProductDTO>(product);
                await _bus.PubSub.PublishAsync(productDto, "product.added");
                return res;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
