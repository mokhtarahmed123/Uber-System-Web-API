using AutoMapper;
using Uber.Uber.Application.DTOs.ReviewsDTOs;
using Uber.Uber.Domain.Entities;

namespace Uber.Uber.Application.Mappings
{
    public class ReviewProfile:Profile
    {
        public ReviewProfile()
        {
            // Create
            CreateMap<CreateReviewDTO, Reviews>()
                   .ForMember(dest => dest.TripID, opt => opt.MapFrom(src => src.TripID))
                   .ForMember(dest => dest.customerID, opt => opt.Ignore()) 
                   .ForMember(dest => dest.DriverID, opt => opt.Ignore())  
                   .ForMember(dest => dest.Massege, opt => opt.MapFrom(src => src.Message))
                   .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
                   .ReverseMap();

            // Update
            CreateMap<UpdateReviewDTO, Reviews>()
                .ForMember(dest => dest.Massege, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
                .ReverseMap();

            // Details
            CreateMap<Reviews, ReviewDetailsDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.customer.UserApp.Name))
                .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => src.Driver.user.Name))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Massege))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.TripID, opt => opt.MapFrom(src => src.TripID))
                .ReverseMap();


        }

    }
}
