using AutoMapper;
using Data;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class ProductRepository : IProductRepository
    {
        private readonly List<ProductDTO> _list = new List<ProductDTO>();
        private readonly ProductsDbContext _context;
        private readonly IMapper _mapper;
        public ProductRepository(ProductsDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _list.Add(new ProductDTO { Count = 1, Name = "aaabbb", Price = 10.1M });
            _list.Add(new ProductDTO { Count = 3, Name = "zzz", Price = 1.9M });
        }
        public Task<ProductsStatDTO> GetStat()
        {
            //return Task.Run(() => new ProductsStatDTO { ItemsCount = _context.Products.Sum(s => s.Count), ProductsCount = _context.Products.Count(), Sum = _context.Products.Sum(s => s.Price) });
            return Task.Run(() => new ProductsStatDTO { ItemsCount = _list.Sum(s => s.Count), ProductsCount = _list.Count(), Sum = _list.Sum(s => s.Price) });
        }

        public Task<IEnumerable<ProductDTO>> GetList(string name)
        {
            //return Task.Run(() => _mapper.Map<IEnumerable<ProductDTO>>(_context.Products).Where(s => s.Name.Contains(name)));
            return Task.Run(() => _list.Where(s => s.Name.Contains(name)));
        }

        public Task<bool> Add(ProductDTO productDto)
        {
            var product = new Product() { Name = productDto.Name, Count = productDto.Count, Price = productDto.Price };
            //_context.Products.Add(product);
            //_context.SaveChanges();
            _list.Add(productDto);
            return Task.Run(() => true);
        }
    }
}
