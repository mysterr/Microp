using Domain.Models;
using Microsoft.AspNetCore.SignalR;
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
        public TestedMessageConsumer(IServiceProvider provider, IHubContext<StatHub> hubContext) : base(provider, hubContext)
        {

        }
    }
    public class MessageQueueTest
    {
        private readonly TestedMessageConsumer _messageConsumer;
        private readonly Mock<IRepository<Product>> _repoMock;
        private readonly Mock<IHubContext<StatHub>> _hubMock;
        private readonly Mock<IClientProxy> _clientProxyMock;
        public MessageQueueTest()
        {
            var serviceMock = new Mock<IServiceProvider>();
            _repoMock = new Mock<IRepository<Product>>();

            _hubMock = new Mock<IHubContext<StatHub>>();
            _clientProxyMock = new Mock<IClientProxy>();
            var clientsMock = new Mock<IHubClients>();
            clientsMock.Setup(clients => clients.All).Returns(_clientProxyMock.Object);
            _hubMock.Setup(x => x.Clients).Returns(() => clientsMock.Object);

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


            _messageConsumer = new TestedMessageConsumer(serviceMock.Object, _hubMock.Object)
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

        [Fact]
        public async void ConsumeCallsUpdateStat()
        {
            var productDto = new ProductDTO();
            await _messageConsumer.ConsumeAsync(productDto);
            _repoMock.Verify(r => r.IncrementStat(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<decimal>()), Times.Once);
        }

        [Fact]
        public async void ConsumeUpdateStatInUI()
        {
            var productDto = new ProductDTO();
            await _messageConsumer.ConsumeAsync(productDto);
            _clientProxyMock.Verify(h => h.SendCoreAsync("UpdateStats", It.Is<object[]>(o => o != null && o.Length == 3), default), Times.Once);
        }

        [Fact]
        public async void IfProductIsNewCountIncrementedByOne()
        {
            var productDto = new ProductDTO();
            productDto.IsNew = true;
            await _messageConsumer.ConsumeAsync(productDto);
            _repoMock.Verify(r => r.IncrementStat(1, It.IsAny<int>(), It.IsAny<decimal>()), Times.Once);
        }
        [Fact]
        public async void IfProductIsNotNewCountNotIncremented()
        {
            var productDto = new ProductDTO();
            productDto.IsNew = false;
            await _messageConsumer.ConsumeAsync(productDto);
            _repoMock.Verify(r => r.IncrementStat(0, It.IsAny<int>(), It.IsAny<decimal>()), Times.Once);
        }
    }
}
