using Domain.Models;
using Moq;
using Products.Database.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Products.Database.Service.Tests.UnitTests
{
    public class TestedMessageConsumer : MessagesConsumer
    {
        public IProductRepository _productRepository;
        public TestedMessageConsumer(IServiceProvider provider): base(provider)
        {

        }
    }
    public class MessageQueueTest
    {
        private readonly TestedMessageConsumer _messageConsumer;
        private readonly Mock<IServiceProvider> _serviceMock;
        private readonly Mock<IProductRepository> _repoMock;
        public MessageQueueTest()
        {
            _serviceMock = new Mock<IServiceProvider>();
            _repoMock = new Mock<IProductRepository>();
            _messageConsumer._productRepository = _repoMock.Object;

            _messageConsumer = new TestedMessageConsumer(_serviceMock.Object);
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
