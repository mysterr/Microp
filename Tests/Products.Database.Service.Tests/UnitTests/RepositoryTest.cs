using AutoMapper;
using Domain.Models;
using EasyNetQ;
using EasyNetQ.FluentConfiguration;
using EasyNetQ.Producer;
using MongoDB.Driver;
using Moq;
using Products.Database.Data;
using Products.Database.Infrastructure;
using Products.Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Services.Tests
{
    public class DRepositoryTest
    {
        private readonly IProductRepository _productRepository;
        private readonly Mock<IBus> _busMock;
        private readonly Mock<IProductsDbContext> _contextMock;
        private readonly Mock<IMapper> _mapperMock;

        public DRepositoryTest()
        {
            var productList = new List<Product>
            {
                new Product { Name = "abcde", Count = 2, Price = 13M },
                new Product { Name = "hello", Count = 8, Price = 2.5M }
            };
            var productStat = new ProductsStat { ItemsCount = 2, Sum = 13M, ProductsCount = 10 };

            _contextMock = new Mock<IProductsDbContext>();

            _contextMock.Setup(c => c.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(productList);
            _contextMock.Setup(c => c.AddAsync(It.IsAny<Product>()))
                .ReturnsAsync(true);
            _contextMock.Setup(c => c.GetStatAsync())
                .ReturnsAsync(productStat);

            _busMock = new Mock<IBus>();
            _mapperMock = new Mock<IMapper>();

            var pubsubMock = new Mock<IPubSub>();
            _busMock.Setup(b => b.PubSub).Returns(pubsubMock.Object);

            _productRepository = new ProductRepository(_contextMock.Object, _busMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetStatReturnsProductsStatDTOAsync()
        {
            var result = await _productRepository.GetStat();

            Assert.IsType<ProductsStat>(result);
            _contextMock.Verify(c => c.GetStatAsync());
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

            Assert.IsAssignableFrom<IEnumerable<Product>>(list);
            _contextMock.Verify(c => c.GetAsync("abc"));
        }
        [Fact]
        public async Task CanAddProduct()
        {
            var product = new Product() {Count = 1, Name = "abc", Price = 10M };
            var res = await _productRepository.Add(product);

            Assert.True(res);
            _contextMock.Verify(c => c.AddAsync(product), Times.Once);
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        [InlineData(int.MinValue)]
        public async Task NegativeOrZeroPriceReturnsFalse(decimal price)
        {
            var product = new Product() {Count = 1, Name = "abc", Price = price };
            var res = await _productRepository.Add(product);

            Assert.False(res);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        [InlineData(int.MinValue)]
        public async Task NegativeOrZeroCountReturnsFalse(int count)
        {
            var product = new Product() {Count = count, Name = "abc", Price = 10M };
            var res = await _productRepository.Add(product);

            Assert.False(res);
        }
        [Fact]
        public async Task WhenProductAddedEventMessageSent()
        {
            var productDto = new ProductDTO() {Count = 1, Name = "abc", Price = 10M };
            var product = new Product() {Count = 1, Name = "abc", Price = 10M };
            _mapperMock.Setup(m => m.Map<ProductDTO>(It.IsAny<object>())).Returns(productDto);

            await _productRepository.Add(product);

            _busMock.Verify(b => b.PubSub.PublishAsync(productDto, It.IsAny<System.Action<IPublishConfiguration>>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
