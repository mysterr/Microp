using Domain.Data;
using Infrastructure;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Services.Tests
{
    public class QServiceTest
    {
        private readonly Mock<IProductRepository> _mock;
        private readonly IProductService _service;
        public QServiceTest()
        {
            _mock = new Mock<IProductRepository>();
            _mock.Setup(r => r.GetStat()).ReturnsAsync(new ProductsStatDTO { ItemsCount = 10, ProductsCount = 2, Sum = 15.5M });
            var productList = new List<ProductDTO>();
            productList.Add(new ProductDTO { Name = "abcde", Count = 2, Price = 13M });
            productList.Add(new ProductDTO { Name = "hello", Count = 8, Price = 2.5M });
            _mock.Setup(r => r.GetList("abc")).ReturnsAsync(productList.Where(s => s.Name.Contains("abc")));
            _mock.Setup(r => r.GetList("")).ReturnsAsync(productList);

            _service = new ProductService(_mock.Object);
        }

        [Fact]
        public async Task GetStatReturnsProductsStatDTOAsync()
        {
            var result = await _service.GetStat();

            Assert.IsType<ProductsStatDTO>(result);
        }

        [Fact]
        public async Task GetStatReturnNotNullAndNotZero()
        {
            var res = await _service.GetStat();

            Assert.NotNull(res);
            Assert.NotEqual(0, res.ItemsCount);
            Assert.NotEqual(0, res.ProductsCount);
            Assert.NotEqual(0, res.Sum);
        }

        [Fact]
        public async Task GetListReturnsListOfProductDTOAsync()
        {
            var list = await _service.GetList("abc");

            Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(list);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("aaa")]
        [InlineData("a")]
        public async Task GetListReturnsListOfProductThatMatchPattern(string searchString)
        {
            var list = await _service.GetList(searchString);

            var listOfProducts = Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(list);

            Assert.True(listOfProducts.All(l => l.Name.IndexOf(searchString) > -1));
        }

        [Fact]
        public async Task GetListWithEmptyParameterReturnsListOfAllProduct()
        {
            var searchString = "";
            var list = await _service.GetList(searchString);

            var listOfProducts = Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(list);

            var stat = await _service.GetStat();

            Assert.Equal(stat.ProductsCount, listOfProducts.Count());
        }

        [Fact]
        public async Task GetListReturnsEmptyResultIfParameterNotCorrespondToAnyProduct()
        {
            var searchString = "qwertyuiop";
            var list = await _service.GetList(searchString);

            var listOfProducts = Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(list);

            Assert.Empty(listOfProducts);
        }
    }
}
