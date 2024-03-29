﻿using Domain.Models;
using Products.Database.Infrastructure;
using Products.Database.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.Database.Domain
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<bool> Add(Product product)
        {
            var res = await _productRepository.Add(product);
            return res;
        }

        public async Task<IEnumerable<Product>> GetList(string name)
        {
            var res = await _productRepository.GetList(name);
            return res;
        }

        public async Task<ProductsStat> GetStat()
        {
            var res = await _productRepository.GetStat();
            return res;
        }
    }
}