﻿using EasyNetQ.AutoSubscribe;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Products.Database.Infrastructure
{
    internal class MessageDispatcher : IAutoSubscriberMessageDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        public MessageDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        void IAutoSubscriberMessageDispatcher.Dispatch<TMessage, TConsumer>(TMessage message, CancellationToken cancellationToken)
        {
            var consumer = _serviceProvider.GetRequiredService<TConsumer>();
            consumer.Consume(message);
        }


        async Task IAutoSubscriberMessageDispatcher.DispatchAsync<TMessage, TConsumer>(TMessage message, CancellationToken cancellationToken)
        {
            var consumer = _serviceProvider.GetRequiredService<TConsumer>();
            await consumer.ConsumeAsync(message);
        }
    }
}