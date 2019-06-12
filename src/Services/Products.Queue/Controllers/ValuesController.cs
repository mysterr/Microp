using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Products.Queue.Infrastructure;
using System.Threading.Tasks;

namespace Products.Queue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }


        [HttpPost]
        public async Task Add(ProductDTO productDTO)
        {
            await _productRepository.Add(productDTO);
        }
    }
}
