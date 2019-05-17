using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Products.Database.Model;
using Microsoft.Extensions.Configuration;
using System.IO;
using Products.Database.Infrastructure;
using Products.Database.Data;
using System.Threading;
using Domain.Models;

namespace Services.Tests
{
    public class DRepositoryTest
    {
        private readonly IProductRepository _productRepository;
        private readonly Mock<ProductsDbContext> _contextMock;
        private readonly Mock<IMapper> _mapperMock;

        public DRepositoryTest()
        {
            //var option = new Mock<IOptions<Settings>>();
            //var myOptions = new MyOptions();
            //option.Setup(o => o.Value).Returns(myOptions.Value);
            var productList = new List<Product>
            {
                new Product { Id = new Guid(), Name = "abcde", Count = 2, Price = 13M },
                new Product { Id = new Guid(), Name = "hello", Count = 8, Price = 2.5M }
            };

            _contextMock = new Mock<ProductsDbContext>();
            var mongoColl = new Mock<IMongoCollection<Product>>();


            var cursorMock = new Mock<IAsyncCursor<Product>>();
            cursorMock.Setup(x => x.Current).Returns(productList);

            mongoColl.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<Product>>(), It.IsAny<FindOptions<Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => cursorMock.Object);

            // need to mock Aggregate
            //  var mongoAggr = new Mock<IAggregateFluent<Product>>();

            _contextMock.Setup(c => c.Products)
                .Returns(mongoColl.Object);

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
