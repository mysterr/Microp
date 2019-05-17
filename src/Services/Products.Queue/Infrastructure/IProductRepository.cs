using Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.Queue.Infrastructure
{
    public interface IProductRepository
    {
        Task Add(ProductDTO product);
    }

}