using Domain.Models;
using EasyNetQ;
using System.Threading.Tasks;

namespace Products.Queue.Infrastructure
{
    public class ProductRepository : IProductRepository
    {
        private readonly IBus _bus;
        public ProductRepository(IBus bus)
        {
            _bus = bus;
        }
        public async Task Add(ProductDTO productDto)
        {
            // send to queue
            await _bus.PubSub.PublishAsync(productDto, c => c.WithTopic("product.add"));
        }
    }
}
