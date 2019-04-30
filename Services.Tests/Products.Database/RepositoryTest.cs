﻿using AutoMapper;
using Data;
using Infrastructure;
using Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MongoDB.Driver;

namespace Services.Tests
{
    public class QRepositoryTest
    {
        private readonly IProductRepository _productRepository;
        private Mock<ProductsDbContext> _contextMock;
        private Mock<IMapper> _mapperMock;

        public QRepositoryTest()
        {
            _contextMock = new Mock<ProductsDbContext>();
            var productList = new List<Product>
            {
                new Product { Id = "1", Name = "abcde", Count = 2, Price = 13M },
                new Product { Id = "2", Name = "hello", Count = 8, Price = 2.5M }
            };
            _mapperMock = new Mock<IMapper>();
            _productRepository = new ProductRepository(_contextMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetStatReturnsProductsStatDTOAsync()
        {
            var result = await _productRepository.GetStat();

            Assert.IsType<ProductsStatDTO>(result);
        }

        [Fact]
        public async Task GetStatReturnNotNullAndNotZero()
        {
            var res = await _productRepository.GetStat();

            Assert.NotNull(res);
            Assert.NotEqual(0, res.ItemsCount);
            Assert.NotEqual(0, res.ProductsCount);
            Assert.NotEqual(0, res.Sum);
        }
        [Fact]
        public async Task GetListReturnsListOfProductDTOAsync()
        {
            var list = await _productRepository.GetList("abc");

            Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(list);
        }
        [Fact]
        public async Task CanAddProduct()
        {
            await _productRepository.Add(new ProductDTO());
        }
    }
}
