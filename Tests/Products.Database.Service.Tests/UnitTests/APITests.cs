﻿using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Products.Database.Controllers;
using Products.Database.Data;
using Products.Database.Infrastructure;
using Products.Database.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Services.Tests
{
    public class DAPITests
    {
        private readonly Mock<IProductService> _mock;
        private readonly ProductsController _controller;
        public DAPITests()
        {
            _mock = new Mock<IProductService>();
            _mock.Setup(r => r.GetStat()).ReturnsAsync(new ProductsStat { ItemsCount = 10, ProductsCount = 2, Sum = 15.5M });
            var productList = new List<Product>();
            productList.Add(new Product { Name = "abcde", Count = 2, Price = 13M });
            productList.Add(new Product { Name = "hello", Count = 8, Price = 2.5M });
            _mock.Setup(r => r.GetList("abc")).ReturnsAsync(productList.Where(s => s.Name.Contains("abc")));
            _mock.Setup(r => r.GetList("")).ReturnsAsync(productList);
            var mapperConf = new MapperConfiguration(cfg =>
                                {
                                    cfg.AddProfile(new DomainProfile());
                                });
            var mapper = mapperConf.CreateMapper();
            _controller = new ProductsController(_mock.Object, mapper);
        }

        [Fact]
        public async Task GetStatReturnsOkObjectResultAsync()
        {
            var result = await _controller.GetStat();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetStatReturnsOkObjectResultOfProductsStatDTO()
        {
            var result = await _controller.GetStat();

            var res = Assert.IsType<OkObjectResult>(result);

            Assert.NotNull(res);
            Assert.NotNull(res.Value);

            Assert.IsType<ProductsStatDTO>(res.Value);
        }

        [Fact]
        public async Task GetStatReturnsActualObjectsCount()
        {
            var statResult = await _controller.GetStat();
            var stat = Assert.IsType<OkObjectResult>(statResult).Value as ProductsStatDTO;

            var listResult = await _controller.GetList("");
            var list = Assert.IsType<OkObjectResult>(listResult).Value as IEnumerable<ProductDTO>;

            Assert.NotEqual(0, stat.ItemsCount);
            Assert.NotEqual(0, stat.ProductsCount);
            Assert.NotEqual(0, stat.Sum);
            Assert.NotEmpty(list);
            Assert.Equal(stat.ProductsCount, list.Count());
            _mock.Verify(r => r.GetStat());
            _mock.Verify(r => r.GetList(""));
        }

        [Fact]
        public async Task GetListReturnsOkObjectResultAsync()
        {
            var result = await _controller.GetList("abc");

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetListReturnsOkObjectOfListOfProductDTOAsync()
        {
            var result = await _controller.GetList("abc");

            var res = Assert.IsType<OkObjectResult>(result);

            var list = res.Value;
            Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(list);
        }

        [Fact]
        public async Task GetListReturnsListOfProductThatMatchPattern()
        {
            var searchString = "abc";
            var result = await _controller.GetList(searchString);

            var res = Assert.IsType<OkObjectResult>(result);

            var list = res.Value;
            var listOfProducts = Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(list);
            Assert.True(listOfProducts.All(l => l.Name.IndexOf(searchString) > -1));
        }
    }
}
