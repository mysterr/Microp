using Domain.Models;
using EasyNetQ.AutoSubscribe;
using Microsoft.Extensions.DependencyInjection;
using Products.Database.Model;
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

        //[AutoSubscriberConsumer(SubscriptionId = "ProductMessageService.AddProduct.Command")]
        [ForTopic("product.add")]
        public async Task ConsumeAsync(ProductDTO productDto, CancellationToken token = default)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Count = productDto.Count,
                Price = productDto.Price
                //Id = new Guid()
            };
            await _productRepository.Add(product);
        }
    }
}
