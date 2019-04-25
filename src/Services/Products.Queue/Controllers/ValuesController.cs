using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Products.Queue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsCAPIController : ControllerBase
    {
        //private readonly IProductService _productService;

        //public ProductsAPIController(IProductService productService)
        //{
        //    this._productService = productService;
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetStat()
        //{
        //    var result = await _productService.GetStat();
        //    return Ok(result);
        //}

        //public async Task<IActionResult> GetList(string name)
        //{
        //    var result = await _productService.GetList(name);
        //    return Ok(result);
        //}
    }
}
