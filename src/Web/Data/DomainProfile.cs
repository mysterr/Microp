using AutoMapper;
using Domain.Models;
using Web.Models;

namespace Web.Data
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<ProductDTO, Product>();
        }
    }
}
