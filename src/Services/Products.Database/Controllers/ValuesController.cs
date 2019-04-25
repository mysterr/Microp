using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Products.Database.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsAPIController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsAPIController(IProductService productService)
        {
            this._productService = productService;
        }

        [Route("GetStat")]
        [HttpGet]
        public async Task<IActionResult> GetStat()
        {
            var result = await _productService.GetStat();
            return Ok(result);
        }

        [Route("GetList")]
        public async Task<IActionResult> GetList(string name)
        {
            var result = await _productService.GetList(name);
            return Ok(result);
        }
    }
}
