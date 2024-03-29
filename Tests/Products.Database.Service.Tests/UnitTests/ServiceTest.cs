﻿using Domain.Models;
using Moq;
using Products.Database.Domain;
using Products.Database.Infrastructure;
using Products.Database.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Services.Tests
{
    public class DServiceTest
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly IProductService _service;
        public DServiceTest()
        {
            _mockRepository = new Mock<IProductRepository>();
            _mockRepository.Setup(r => r.GetStat()).ReturnsAsync(new ProductsStat { ItemsCount = 10, ProductsCount = 2, Sum = 15.5M });
            var productList = new List<Product>
            {
                new Product { Id = new System.Guid(), Name = "abcde", Count = 2, Price = 13M },
                new Product { Id = new System.Guid(), Name = "hello", Count = 8, Price = 2.5M }
            };
            _mockRepository.Setup(r => r.GetList("abc")).ReturnsAsync(productList.Where(s => s.Name.Contains("abc")));
            _mockRepository.Setup(r => r.GetList("")).ReturnsAsync(productList);

            _service = new ProductService(_mockRepository.Object);
            //_service = new ProductService(new ProductRepository());
        }

        [Fact]
        public async Task GetStatReturnsProductsStatDTOAsync()
        {
            var result = await _service.GetStat();

            Assert.IsType<ProductsStat>(result);
        }

        [Fact]
        public async Task GetStatReturnNotNullAndNotZero()
        {
            var res = await _service.GetStat();

            Assert.NotNull(res);
            Assert.NotEqual(0, res.ItemsCount);
            Assert.NotEqual(0, res.ProductsCount);
            Assert.NotEqual(0, res.Sum);
            _mockRepository.Verify(r => r.GetStat());
        }

        [Fact]
        public async Task GetListReturnsListOfProductDTOAsync()
        {
            var list = await _service.GetList("abc");

            Assert.IsAssignableFrom<IEnumerable<Product>>(list);
            _mockRepository.Verify(r => r.GetList("abc"));
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("aaa")]
        [InlineData("a")]
        public async Task GetListReturnsListOfProductThatMatchPattern(string searchString)
        {
            var list = await _service.GetList(searchString);

            var listOfProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(list);

            Assert.True(listOfProducts.All(l => l.Name.IndexOf(searchString) > -1));
        }

        [Fact]
        public async Task GetListWithEmptyParameterReturnsListOfAllProduct()
        {
            var searchString = "";
            var list = await _service.GetList(searchString);

            var listOfProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(list);

            var stat = await _service.GetStat();

            Assert.Equal(stat.ProductsCount, listOfProducts.Count());
        }

        [Fact]
        public async Task GetListReturnsEmptyResultIfParameterNotCorrespondToAnyProduct()
        {
            var searchString = "!#$%@^%$";
            var list = await _service.GetList(searchString);

            var listOfProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(list);

            Assert.Empty(listOfProducts);
        }
        [Fact]
        public async Task AddResultProductAdded()
        {
            var list = await _service.GetList("test");
            var testCnt = list.Count();
            var product = new Product { Name = "test", Count = 5, Price = 4M };
            await _service.Add(product);
            list = await _service.GetList("test");
            //Assert.Equal(testCnt+1, list.Count());
            _mockRepository.Verify(r => r.Add(product));
        }
    }
}
