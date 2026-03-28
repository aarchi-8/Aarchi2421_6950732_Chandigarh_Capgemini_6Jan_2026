using AutoMapper;
using OrderApi.DTOs;
using OrderApi.Models;

namespace OrderApi.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<OrderDto, Order>();
        }
    }
}
