using Domain.Models;
using Moq;
using Products.Queue.Controllers;
using Products.Queue.Infrastructure;
using Xunit;

namespace Products.Queue.Service.Test.UnitTests
{
    public class QAPITests
    {
        private readonly ProductsController _productController;
        private readonly Mock<IProductRepository> _mock;

        public QAPITests()
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
