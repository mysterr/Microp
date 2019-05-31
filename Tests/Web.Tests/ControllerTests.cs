using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Web.Controllers;
using Web.Infrastructure;
using Web.Models;
using Web.ViewModels;
using Xunit;

namespace Web.Tests
{
    public class ControllerTests
    {
        private readonly HomeController _controller;
        private readonly Mock<IRepository<Product>> _mock;
        public ControllerTests()
        {
            _mock = new Mock<IRepository<Product>>();
            _mock.Setup(r => r.GetCount()).ReturnsAsync(1);
            _mock.Setup(r => r.GetTotal()).ReturnsAsync(10);
            _mock.Setup(r => r.GetSum()).ReturnsAsync(33.33M);
            _mock.Setup(r => r.Get("Comp")).ReturnsAsync(new List<Product> { new Product { Name = "Computer", Count = 10, Price = 33.33M } });
            //_mock.Setup(r => r.Get(It.Is<string>(s=>s.Contains("omp")))).ReturnsAsync(new List<Product> { new Product { Name = "Computer", Count = 10, Price = 33.33M } });
            _controller = new HomeController(_mock.Object);

        }
        [Fact]
        public async Task Index_ReturnsAViewResult_WithProductsSummary()
        {
            var result = await _controller.Index();

            // Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ProductsSummary>(
                viewResult.ViewData.Model);
        }

        [Fact]
        public async Task ProductsSummaryHas_ProductsCount_ItemsCount_Sum()
        {
            var result = await _controller.Index() as ViewResult;
            var productSummaryView = Assert.IsAssignableFrom<ProductsSummary>(result.ViewData.Model);

            // Assert
            //Assert.Equal(3, ((ProductsSummary)result.ViewData.Model).ProductsCount);
            Assert.IsType<int>(productSummaryView.ProductsCount);
            Assert.IsType<int>(productSummaryView.ItemsCount);
            Assert.IsType<decimal>(productSummaryView.Sum);

            Assert.Equal(1, productSummaryView.ProductsCount);
            Assert.Equal(10, productSummaryView.ItemsCount);
            Assert.Equal(33.33M, productSummaryView.Sum);
            _mock.Verify(r => r.GetCount());
            _mock.Verify(r => r.GetTotal());
            _mock.Verify(r => r.GetSum());
        }

        [Fact]
        public async Task Search_ReturnsListOfProducts()
        {
            var result = await _controller.Search("Comp");

            // Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<IEnumerable<Product>>(
                viewResult.Model);
            _mock.Verify(r => r.Get("Comp"));
        }

        [Fact]
        public async Task Search_ReturnsOneResult()
        {
            var result = await _controller.Search("Comp");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var listResult = Assert.IsAssignableFrom<IEnumerable<Product>>(
                viewResult.Model);
            Assert.Single(listResult);
        }
        [Fact]
        public async Task Search_ReturnsCorrectResult()
        {
            var result = await _controller.Search("Comp");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var listResult = Assert.IsAssignableFrom<IEnumerable<Product>>(
                viewResult.Model);
            Assert.All(listResult, s => s.Name.Contains("Comp"));
        }
        [Fact]
        public async Task Search_ReturnsNotFoundWhenNotFound()
        {
            var result = await _controller.Search("xxx");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(viewResult.ViewData.ContainsKey("message"));
            Assert.Equal("Product not found", viewResult.ViewData["message"]);
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("22")]
        public async Task Search_ReturnsTooShortWhenParamShorterThan3(string value)
        {
            var result = await _controller.Search(value);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(viewResult.ViewData.ContainsKey("message"));
            Assert.Equal("Search string is too short", viewResult.ViewData["message"]);
        }

        [Fact]
        public async Task Search_ReturnsBadRequestWhenParamIsNull()
        {
            var result = await _controller.Search(null);

            // Assert
            var viewResult = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void SearchGet_ReturnsSearchString()
        {
            var result = _controller.Search();

            // Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(viewResult.ViewData.ContainsKey("searchstring"));
            Assert.NotNull(viewResult.ViewData["searchstring"]);
            Assert.IsType<string>(viewResult.ViewData["searchstring"]);
        }
        [Fact]
        public void AddCommandReturnsViewResultAsync()
        {
            var result = _controller.Add();
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public async Task AddCommandWithParametersReturnsViewResultAsync()
        {
            var product = new Product("Test prod", 10, 15.5M);

            var result = await _controller.Add(product);
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            _mock.Verify(r => r.Create(product));
        }
        [Fact]
        public async Task Add_ReturnsBadRequestWhenParamIsNull()
        {
            var result = await _controller.Add(null);

            // Assert
            var viewResult = Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task AddReturnsBadRequestIfModelStateIsError()
        {
            var product = new Product("", 10, 15.5M);
            _controller.ModelState.AddModelError("Product", "Required");
            var result = await _controller.Add(product);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<string>(badRequestResult.Value);
        }
        [Fact]
        public async Task AddReturnsErrorIfNameIsEmpty()
        {
            var product = new Product("", 10, 15.5M);

            var result = await _controller.Add(product);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(viewResult.ViewData.ContainsKey("error"));
            Assert.Equal("Name should not be empty", viewResult.ViewData["error"]);
        }
        [Fact]
        public async Task AddReturnsErrorIfNameIsNull()
        {
            var product = new Product(null, 10, 15.5M);

            var result = await _controller.Add(product);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Name should not be null", viewResult.ViewData["error"]);
        }
        [Fact]
        public async Task AddReturnsErrorIfCountIsZero()
        {
            var product = new Product("Test", 0, 15.5M);

            var result = await _controller.Add(product);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Count should be more than 0", viewResult.ViewData["error"]);
        }
        [Fact]
        public async Task AddReturnsErrorIfPriceIsZero()
        {
            var product = new Product("Test", 10, 0);

            var result = await _controller.Add(product);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Price should be more than 0", viewResult.ViewData["error"]);
        }
        [Fact]
        public async Task AddReturnsNoErrorsIfAllOk()
        {
            var product = new Product("Test", 10, 15.5M);

            var result = await _controller.Add(product);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("", viewResult.ViewData["error"]);
        }
        [Theory]
        [InlineData("Phone")]
        [InlineData("Pen")]
        [InlineData("Knife")]
        public async Task AddReturnsProductAddedIfAllOk(string name)
        {
            var product = new Product(name, 10, 15.5M);

            var result = await _controller.Add(product);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(viewResult.ViewData.ContainsKey("message"));
            Assert.Equal($"Product {name} is added sucessfully", viewResult.ViewData["message"]);
        }
        [Fact(Skip = "go to integration")]
        public async Task AddProductActuallyAdded()
        {
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var controller = new HomeController(new ProductRepository(mockHttpClientFactory.Object, new Mock<IConfiguration>().Object));
            var product = new Product("Unic product name", 10, 15.5M);

            var result = await controller.Add(product);
            var search = await controller.Search("Unic product name");
            var viewResult = Assert.IsType<ViewResult>(search);
            var listResult = Assert.IsAssignableFrom<IEnumerable<Product>>(
                viewResult.Model);
            Assert.NotEmpty(listResult);
        }
    }

}
