using Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Products.Database.Infrastructure;
using System;
using Xunit;

namespace Products.Database.Service.Tests.UnitTests
{
    public class TestedMessageConsumer : MessagesConsumer
    {
        public IProductRepository _productRepository;
        public TestedMessageConsumer(IServiceProvider provider) : base(provider)
        {

        }
    }
    public class MessageQueueTest
    {
        private readonly TestedMessageConsumer _messageConsumer;
        private readonly Mock<IProductRepository> _repoMock;
        public MessageQueueTest()
        {
            var serviceMock = new Mock<IServiceProvider>();
            _repoMock = new Mock<IProductRepository>();

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(serviceMock.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            serviceMock
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);

            serviceMock
                .Setup(x => x.GetService(typeof(IProductRepository)))
                .Returns(_repoMock.Object);


            _messageConsumer = new TestedMessageConsumer(serviceMock.Object);
            _messageConsumer._productRepository = _repoMock.Object;
        }
        [Fact]
        public async void CanReceiveMessage()
        {
            var productDto = new ProductDTO();
            await _messageConsumer.ConsumeAsync(productDto);
            _repoMock.Verify(r => r.Add(productDto), Times.Once);
        }
    }
}
