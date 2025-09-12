using AutoMapper;
using Uber.Uber.Domain.Entities;

namespace Uber.Uber.Application.Mappings
{
    public class ItemProfile : Profile
    {
        public ItemProfile()
        {
            CreateMap<CreateandUpdateItemDTO, Item>().ReverseMap();

            CreateMap<Item, GetItemDTO>().ReverseMap();

            CreateMap<CreateandUpdateItemDTO, Item>()
                .ForMember(dest => dest.MerchantID, opt => opt.Ignore()) 
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore()) 
                .ReverseMap();


            CreateMap<Item, ItemListDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.category.Name))
                .ForMember(dest => dest.MerchentEmail, opt => opt.MapFrom(src => src.merchant.UserApp.Email)).
                ForMember(dest=>dest.CategoryName , opt=>opt.MapFrom(src=>src.category.Name));

            CreateMap<Item, GetItemDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.category.Name))
                .ForMember(dest => dest.MerchantEmail, opt => opt.MapFrom(src => src.merchant.UserApp.Email))
                .ForMember(dest => dest.MerchantName, opt => opt.MapFrom(src => src.merchant.UserApp.Name));
        }
    }
}
