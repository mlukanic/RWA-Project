﻿using AutoMapper;
using MarketplaceClassLibrary.Models;
using MarketplaceMvc.ViewModels;

namespace MarketplaceMvc.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserVM>();
            CreateMap<ItemType, ItemTypeVM>();
            CreateMap<Tag, TagVM>();
            CreateMap<ItemTag, ItemTagVM>();
            CreateMap<Item, ItemVM>();
        }
    }
}
