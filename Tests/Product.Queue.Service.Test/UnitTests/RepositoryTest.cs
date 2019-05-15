using EasyNetQ;
using Moq;
using Products.Queue.Data;
using Products.Queue.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Product.Queue.Service.Test.UnitTests
{
    public class RepositoryTest
    {
        public RepositoryTest()
        {

        }

        [Fact]
        public async void CanAddProduct()
        {
            var busMock = new Mock<IBus>();
            var repository = new ProductRepository(busMock.Object);
            var productDTO = new ProductDTO
            {
                Name = "Test",
                Count = 3,
                Price = 9M
            };
            await repository.Add(productDTO);
            busMock.Verify(b => b.PublishAsync(productDTO));
        }
    }
}
