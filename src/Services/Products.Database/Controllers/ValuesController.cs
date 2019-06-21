using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Products.Database.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.Database.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;


        public ProductsController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetStat()
        {
            try
            {
                var productStat = await _productService.GetStat();
                var result = _mapper.Map<ProductsStatDTO>(productStat);
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
                var products = await _productService.GetList(name);
                var result = _mapper.Map<IEnumerable<ProductDTO>>(products);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
