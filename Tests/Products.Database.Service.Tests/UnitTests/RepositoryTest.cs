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

            _contextMock.Setup(c => c.SearchAsync(It.IsAny<string>()))
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
            _contextMock.Verify(c => c.SearchAsync("abc"));
        }

        [Fact]
        public async Task CanAddProduct()
        {
            var product = new Product() {Count = 1, Name = "abc", Price = 10M };
            var productDto = new ProductDTO() {Count = 1, Name = "abc", Price = 10M };
            _mapperMock.Setup(m => m.Map<ProductDTO>(It.IsAny<object>())).Returns(productDto);
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

        [Theory]
        [InlineData(9, "abc", 15)]
        [InlineData(5, "test", 25)]
        public async Task WhenProductAddedEventMessageSent(int count, string name, decimal price)
        {
            var productDto = new ProductDTO() {Count = count, Name = name, Price = price };
            var product = new Product() {Count = count, Name = name, Price = price };
            _mapperMock.Setup(m => m.Map<ProductDTO>(It.IsAny<object>())).Returns(productDto);

            await _productRepository.Add(product);

            _busMock.Verify(b => b.PubSub.PublishAsync(It.IsAny<ProductDTO>(), It.IsAny<System.Action<IPublishConfiguration>>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Theory]
        [InlineData(9, "abc", 15)]
        [InlineData(5, "test", 25)]
        public async Task WhenProductAddedCheckIfProductAlreadyExists(int count, string name, decimal price)
        {
            var productDto = new ProductDTO() {Count = count, Name = name, Price = price };
            var product = new Product() {Count = count, Name = name, Price = price };
            _mapperMock.Setup(m => m.Map<ProductDTO>(It.IsAny<object>())).Returns(productDto);

            await _productRepository.Add(product);

            _contextMock.Verify(c => c.GetAsync(name));
        }

        [Theory]
        [InlineData(9, "abc", 15)]
        [InlineData(5, "test", 25)]
        public async Task WhenProductAddedAndProductAlreadyExistsCountIsZero(int count, string name, decimal price)
        {
            var productDto = new ProductDTO() {Count = 0, Name = name, Price = price };
            var product = new Product() {Count = count, Name = name, Price = price };
            _mapperMock.Setup(m => m.Map<ProductDTO>(It.IsAny<object>())).Returns(productDto);
            _contextMock.Setup(c => c.GetAsync(name)).ReturnsAsync(new List<Product>() { product });

            await _productRepository.Add(product);

            _busMock.Verify(b => b.PubSub.PublishAsync(productDto, It.IsAny<System.Action<IPublishConfiguration>>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Theory]
        [InlineData(9, "abc", 15)]
        [InlineData(5, "test", 25)]
        public async Task WhenProductAddedAndProductNotExistsCountIsOne(int count, string name, decimal price)
        {
            var productDto = new ProductDTO() {Count = 1, Name = name, Price = price };
            var product = new Product() {Count = count, Name = name, Price = price };
            _mapperMock.Setup(m => m.Map<ProductDTO>(It.IsAny<object>())).Returns(productDto);
            _contextMock.Setup(c => c.GetAsync(name)).ReturnsAsync(new List<Product>());

            await _productRepository.Add(product);

            _busMock.Verify(b => b.PubSub.PublishAsync(productDto, It.IsAny<System.Action<IPublishConfiguration>>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
