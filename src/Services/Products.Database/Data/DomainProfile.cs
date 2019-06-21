using AutoMapper;
using Domain.Models;
using Products.Database.Model;

namespace Products.Database.Data
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<Product, ProductDTO>();
            //CreateMap<ProductDTO, Product>();
            CreateMap<ProductsStat, ProductsStatDTO>();
            CreateMap<ProductsStatDTO, ProductsStat>();
        }
    }
}
