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
        }
    }
}
