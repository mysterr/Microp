using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Models;
using EasyNetQ.AutoSubscribe;
using Microsoft.Extensions.DependencyInjection;
using Web.Models;

namespace Web.Infrastructure
{
     public class MessagesConsumer : IConsumeAsync<ProductDTO>//, IConsume<ProductDTO>
    {
        private readonly IRepository<Product> _productRepository;

        public MessagesConsumer(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                _productRepository = services.GetRequiredService<IRepository<Product>>();
            }
        }

        //[AutoSubscriberConsumer(SubscriptionId = "ProductMessageService.AddProduct.Event")]
        [ForTopic("product.added")]
        public Task ConsumeAsync(ProductDTO productDto, CancellationToken token = default)
        {
            return _productRepository.IncrementStat(productDto.IsNew ? 1 : 0, productDto.Count, productDto.Price);
        }
        //public void Consume(ProductDTO productDto, CancellationToken token = default)
        //{
        //    Task.Run(async () => await _productRepository.IncrementStat(productDto.IsNew ? 1 : 0, productDto.Count, productDto.Price));
        //}
    }
}