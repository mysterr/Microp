using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Products.Queue.Infrastructure;

namespace Products.Queue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            this._productRepository = productRepository;
        }


        [HttpPost]
        public async Task Add(ProductDTO productDTO)
        {
            await _productRepository.Add(productDTO);
        }
    }
}
