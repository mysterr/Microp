using Domain.Models;
using EasyNetQ.AutoSubscribe;
using EasyNetQ.Consumer;
using Products.Database.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Products.Database.Infrastructure
{
    public class MessagesConsumer : IConsumeAsync<ProductDTO>
    {
        private readonly IProductRepository _productRepository;
        public MessagesConsumer(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        [AutoSubscriberConsumer(SubscriptionId = "ProductMessageService.AddProduct")]
        [ForTopic("product.add")]
        public async Task ConsumeAsync(ProductDTO product)
        {
            await _productRepository.Add(product);
        }
    }
}
