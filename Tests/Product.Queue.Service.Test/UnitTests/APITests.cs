using Products.Queue.Data;
using Products.Queue.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Products.Queue.Infrastructure;
using Moq;

namespace Services.Tests.Products.Queue
{
    public class APITests
    {
        private readonly ProductsController _productController;
        private readonly Mock<IProductRepository> _mock;

        public APITests()
        {
            _mock = new Mock<IProductRepository>();
            _productController = new ProductsController(_mock.Object);
        }

        [Fact]
        public async void CanAddProduct()
        {
            var productDTO = new ProductDTO
            {
                Name = "Test",
                Count = 3,
                Price = 9M
            };
            await _productController.Add(productDTO);
            _mock.Verify(m => m.Add(productDTO));
        }
    }
}
