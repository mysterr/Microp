using AutoMapper;
using Products.Database.Data;
using Microsoft.EntityFrameworkCore;
using Products.Database.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Products.Database.Infrastructure
{
    public class ProductRepository : IProductRepository
    {
        //private readonly List<ProductDTO> _list = new List<ProductDTO>();
        private readonly ProductsDbContext _context;
        private readonly IMapper _mapper;
        public ProductRepository(ProductsDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;

            //_list.Add(new ProductDTO { Count = 1, Name = "aaabbb", Price = 10.1M });
            //_list.Add(new ProductDTO { Count = 3, Name = "zzz", Price = 1.9M });
        }
        public async Task<ProductsStatDTO> GetStat()
        {
            if (0 == await _context.Products.CountDocumentsAsync(_ => true))
            {
                return new ProductsStatDTO
                {
                    ItemsCount = 0,
                    ProductsCount = 0,
                    Sum = 0
                };
            }
            try
            {
                var total = await _context.Products.Aggregate().Group(_ => true,
                        g => new
                        {
                            itemsCount = g.Count(),
                            productsCount = g.Sum(f => f.Count),
                            productsSum = g.Sum(f => f.Price)
                        }).FirstAsync();

                return new ProductsStatDTO
                {
                    ItemsCount = total.itemsCount,
                    ProductsCount = total.productsCount,
                    Sum = total.productsSum
                };
            }
            catch (Exception ex)
            {

                throw new DatabaseErrorException(ex);
            }



            //return Task.Run(() => new ProductsStatDTO { ItemsCount = _context.Products.Sum(s => s.Count), ProductsCount = _context.Products.Count(), Sum = _context.Products.Sum(s => s.Price) });
            //return Task.Run(() => new ProductsStatDTO { ItemsCount = _list.Sum(s => s.Count), ProductsCount = _list.Count(), Sum = _list.Sum(s => s.Price) });
        }

        public async Task<IEnumerable<ProductDTO>> GetList(string name)
        {
            if (0 == await _context.Products.CountDocumentsAsync(_ => true))
            {
                return new List<ProductDTO>();
            }
            var filter = Builders<Product>.Filter.Where(f => f.Name.Contains(name ?? ""));
            try
            {
                var products = await _context.Products.FindAsync(filter);
                if (products.Any())
                    return _mapper.Map<IEnumerable<ProductDTO>>(products);
                else
                    return new List<ProductDTO>();
            }
            catch (Exception ex)
            {
                throw new DatabaseErrorException(ex);
            }
            //var res = _mapper.Map<IEnumerable<ProductDTO>>(_context.Products).Where(s => s.Name.Contains(name));
            //return Task.Run(() => _list.Where(s => s.Name.Contains(name)));
        }

        public async Task<bool> Add(ProductDTO productDto)
        {
            var product = new Product() { Name = productDto.Name, Count = productDto.Count, Price = productDto.Price };
            try
            {
                await _context.Products.InsertOneAsync(product);
            }
            catch 
            {
                return false;
            }
            //_list.Add(productDto);
            return true;
        }
    }
}
