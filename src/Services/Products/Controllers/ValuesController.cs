using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Data;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Products.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsAPIController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsAPIController(IProductRepository productRepository)
        {
            this._productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetStat()
        {
            var result = await _productRepository.GetStat();
            return Ok(result);
        }

        public async Task<IActionResult> GetList(string name)
        {
            var result = await _productRepository.GetList(name);
            return Ok(result);
        }
    }
}
