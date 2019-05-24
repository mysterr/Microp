using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web.Infrastructure;
using Web.Models;
using Web.ViewModels;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Product> _repo;
        public HomeController(IRepository<Product> repo)
        {
            _repo = repo;
        }
        public async Task<IActionResult> Index()
        {
            var productSummary = new ProductsSummary()
            {
                ItemsCount = await _repo.GetTotal(),
                ProductsCount = await _repo.GetCount(),
                Sum = await _repo.GetSum()
            };

            return View(productSummary);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchstring)
        {
            if (searchstring == null)
                return BadRequest("error occured");

            if (searchstring.Length < 3)
            {
                ViewData["message"] = "Search string is too short";
                return View();
            }
            else
            {
                var result = await _repo.Get(searchstring);
                if (result == null || result.Count() == 0)
                    ViewData["message"] = "Product not found";

                return View(result);
            }
        }

        public IActionResult Search()
        {
            ViewData["searchstring"] = "test";
            return View();
        }

        public IActionResult Add()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm]Product product)
        {
            if (!ModelState.IsValid || product == null)
                return BadRequest("Product required");

            ViewData["error"] = "";
            ViewData["message"] = "";

            if (product.Name == null)
                ViewData["error"] = "Name should not be null";
            else if (product.Name == "")
                ViewData["error"] = "Name should not be empty";
            else if (product.Count == 0)
                ViewData["error"] = "Count should be more than 0";
            else if (product.Price == 0)
                ViewData["error"] = "Price should be more than 0";
            else
            {
                await _repo.Create(product);
                ViewData["message"] = $"Product {product.Name} is added sucessfully";
            }

            return View(product);
        }
    }
}
