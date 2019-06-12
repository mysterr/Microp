using Microsoft.AspNetCore.Mvc;
using Products.Database.Infrastructure;
using System;
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
            try
            {
                var result = await _productService.GetStat();
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetList(string name)
        {
            try
            {
                var result = await _productService.GetList(name);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
