using AutoMapper;
using Domain.Models;
using Products.Database.Model;

namespace Products.Database.Data
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.IsNew, opt => opt.Ignore());
            //CreateMap<ProductDTO, Product>();
            CreateMap<ProductsStat, ProductsStatDTO>();
            CreateMap<ProductsStatDTO, ProductsStat>();
        }
    }
}
