using AutoMapper;
using Marketplace.Dtos;
using MarketplaceClassLibrary.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Marketplace.Mapping
{
    public class MappingProfile : Profile
    {
        protected MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<Item, ItemDto>();
        }
    }
}
