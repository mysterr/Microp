﻿using AutoMapper;
using Data;
using Microsoft.EntityFrameworkCore;
using Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
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
            //return Task.Run(() => new ProductsStatDTO { ItemsCount = _context.Products.Sum(s => s.Count), ProductsCount = _context.Products.Count(), Sum = _context.Products.Sum(s => s.Price) });
            //return Task.Run(() => new ProductsStatDTO { ItemsCount = _list.Sum(s => s.Count), ProductsCount = _list.Count(), Sum = _list.Sum(s => s.Price) });
        }

        public async Task<IEnumerable<ProductDTO>> GetList(string name)
        {
            var filter = Builders<Product>.Filter.Where(f=> f.Name.Contains(name));
            try
            {
                var products = await _context.Products.Find(filter).ToListAsync();
                return _mapper.Map<IEnumerable<ProductDTO>>(products);
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
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
            catch (Exception e)
            {
                return false;
            }
            //_list.Add(productDto);
            return true;
        }
    }
}
