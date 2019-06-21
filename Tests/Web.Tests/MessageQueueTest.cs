using Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using Web.Infrastructure;
using Web.Models;
using Xunit;

namespace Web.Tests.UnitTests
{
    public class TestedMessageConsumer : MessagesConsumer
    {
        public IRepository<Product> _productRepository;
        public TestedMessageConsumer(IServiceProvider provider) : base(provider)
        {

        }
    }
    public class MessageQueueTest
    {
        private readonly TestedMessageConsumer _messageConsumer;
        private readonly Mock<IRepository<Product>> _repoMock;
        public MessageQueueTest()
        {
            var serviceMock = new Mock<IServiceProvider>();
            _repoMock = new Mock<IRepository<Product>>();

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
                .Setup(x => x.GetService(typeof(IRepository<Product>)))
                .Returns(_repoMock.Object);


            _messageConsumer = new TestedMessageConsumer(serviceMock.Object)
            {
                _productRepository = _repoMock.Object
            };
        }
        [Fact]
        public async void CanReceiveMessage()
        {
            var productDto = new ProductDTO();
            await _messageConsumer.ConsumeAsync(productDto);
        }
    }
}
