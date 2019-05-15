﻿using EasyNetQ;
using Products.Queue.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task Add(ProductDTO product)
        {
            // send to queue
            await _bus.PublishAsync(product);
        }
    }
}
