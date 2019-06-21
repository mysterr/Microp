using System.Threading;
using Domain.Models;
using EasyNetQ;
using EasyNetQ.FluentConfiguration;
using EasyNetQ.Producer;
using Moq;
using Moq.Protected;
using Products.Queue.Infrastructure;
using Xunit;

namespace Products.Queue.Service.Test.UnitTests
{
    public class QRepositoryTest
    {
        public QRepositoryTest()
        {

        }

        [Fact]
        public async void CanAddProduct()
        {
            var busMock = new Mock<IBus>();
            var pubsubMock = new Mock<IPubSub>();
            busMock.Setup(b => b.PubSub).Returns(pubsubMock.Object);
            var repository = new ProductRepository(busMock.Object);
            var productDTO = new ProductDTO
            {
                Name = "Test",
                Count = 3,
                Price = 9M
            };
            await repository.Add(productDTO);
            busMock.Verify(p => p.PubSub.PublishAsync(productDTO, It.IsAny<System.Action<IPublishConfiguration>>(), It.IsAny<CancellationToken>()), Times.Once());
            //pubsubMock.Verify(p => p.PublishAsync(productDTO, It.IsAny<System.Action<IPublishConfiguration>>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
