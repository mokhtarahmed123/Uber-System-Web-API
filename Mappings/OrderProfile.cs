using AutoMapper;
using Uber.Uber.Application;
using Uber.Uber.Application.DTOs.OrderDTO;
using Uber.Uber.Domain.Entities;

namespace Uber.Uber
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, CreateOrUpdateOrder>()
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.item.Name))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.user.UserApp.Email))
                .ForMember(dest => dest.MerchantEmail, opt => opt.MapFrom(src => src.merchant.UserApp.Email))
                .ForMember(dest=>dest.TotalAmount , opt=>opt.MapFrom(a=>a.totalAmount))
                .ReverseMap()
                .ForMember(dest => dest.item, opt => opt.Ignore())
                .ForMember(dest => dest.user, opt => opt.Ignore())
                .ForMember(dest => dest.merchant, opt => opt.Ignore());



            // Order Details Mapping
            CreateMap<Order, OrderDetailsDTO>()
                .ForMember(dest=>dest.OrderId , opt=>opt.MapFrom(src=>src.Id))
                .ForMember(dest => dest.CustomerEmail, op => op.MapFrom(src => src.user.UserApp.Email))
                .ForMember(dest => dest.MerchantEmail, op => op.MapFrom(src => src.user.UserApp.Email))
                .ForMember(dest => dest.ItemName, op => op.MapFrom(src => src.item.Name))
                .ForMember(dest => dest.ItemPrice, op => op.MapFrom(src => src.item.Price))
                             .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(a => a.totalAmount))

                .ForMember(dest => dest.TotalAmount, op => op.MapFrom(src => src.item.Quantity))
                .ForMember(dest => dest.TotalPrice, op => op.MapFrom(src => src.item.Price * src.item.Quantity))
                .ReverseMap();

            // Order List DTO
            CreateMap<Order, OrderListDTO>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.item.Name))
                .ForMember(dest => dest.MerchantEmail, opt => opt.MapFrom(src => src.merchant.UserApp.Email))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.user.UserApp.Email))
                                            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(a => a.totalAmount))

                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.item.Price * src.item.Quantity));


            // Order By Customer DTO
            CreateMap<Order, OrderByCustomerDTO>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ItemName, op => op.MapFrom(src => src.item.Name))
                .ForMember(dest => dest.CustomerEmail, op => op.MapFrom(src => src.user.UserApp.Email))
                .ForMember(dest => dest.Payment, op => op.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.TotalAmount , op => op.MapFrom(src => src.totalAmount));

        }

    }
}
