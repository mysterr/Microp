using Domain.Models;
using EasyNetQ.AutoSubscribe;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Products.Database.Infrastructure
{
    public class MessagesConsumer : IConsumeAsync<ProductDTO>
    {
        private readonly IProductRepository _productRepository;

        public MessagesConsumer(IServiceProvider serviceProvider)
        {
            var services = serviceProvider.CreateScope().ServiceProvider;
            _productRepository = services.GetRequiredService<IProductRepository>();
        }

        //        [AutoSubscriberConsumer(SubscriptionId = "ProductMessageService.AddProduct")]
        [ForTopic("product.add")]
        public async Task ConsumeAsync(ProductDTO product, CancellationToken token = default)
        {
            await _productRepository.Add(product);
        }
    }
}
