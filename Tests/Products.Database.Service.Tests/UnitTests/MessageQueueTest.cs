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
    public class MessageQueueTest
    {
        private readonly MessagesConsumer _messageConsumer;
        private readonly Mock<IProductRepository> _repoMock;
        public MessageQueueTest()
        {
            _repoMock = new Mock<IProductRepository>();
            _messageConsumer = new MessagesConsumer(_repoMock.Object);
        }
        [Fact]
        public async void CanReceiveMessage()
        {
            var productDto = new ProductDTO();
            await _messageConsumer.ConsumeAsync(productDto);
            _repoMock.Verify(r => r.Add(productDto));
        }
    }
}
