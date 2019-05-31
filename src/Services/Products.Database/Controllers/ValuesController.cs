using Microsoft.AspNetCore.Mvc;
using Products.Database.Infrastructure;
using System.Threading.Tasks;

namespace Products.Database.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            this._productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStat()
        {
            var result = await _productService.GetStat();
            return Ok(result);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetList(string name)
        {
            var result = await _productService.GetList(name);
            return Ok(result);
        }
    }
}
