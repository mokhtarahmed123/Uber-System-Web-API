using AutoMapper;
using Uber.Uber.Application.DTOs.DeliveryDTOs;
using Uber.Uber.Domain.Entities;

namespace Uber.Uber.Application.Mappings
{
    public class DeliveryProfile:Profile
    {
        public DeliveryProfile()
        {
            CreateMap<Delivery, CreateDeliveryDTO>()
                .ForMember(dest => dest.DriverEmail, opt => opt.MapFrom(src => src.Driver.user.Email))
                .ForMember(dest=>dest.TripId , opt=>opt.MapFrom(a=>a.trip.ID))
                .ReverseMap()
                .ForMember(dest => dest.Driver, opt => opt.Ignore())
                .ForMember(dest=>dest.trip , opt=>opt.Ignore());




            CreateMap<Delivery, UpdateDeliveryDTO>()
                .ForMember(dest => dest.DriverEmail, opt => opt.MapFrom(src => src.Driver.user.Email))
                .ForMember(dest=>dest.TripId , opt=>opt.MapFrom(opt=>opt.trip.ID))
                .ReverseMap()
                .ForMember(dest => dest.Driver, opt => opt.Ignore());

 

            CreateMap<Delivery, DeliveryDetailsDTO>()
                .ForMember(dest => dest.DelivaryEmail, opt => opt.MapFrom(src => src.Driver.user.Email))
                .ForMember(dest => dest.DelivaryName, opt => opt.MapFrom(src => src.Driver.user .Name))
                .ForMember(dest=>dest.TripID , opt=>opt.MapFrom(src=>src.TripId));

            CreateMap<Delivery, ListDeliveryDTO>()
                .ForMember(dest => dest.DriverEmail, opt => opt.MapFrom(src => src.Driver.user.Email))
                .ForMember(dest => dest.DelivaryName, opt => opt.MapFrom(src => src.Driver.user.Name))
                .ForMember(dest=>dest.TripId , opt=>opt.MapFrom(src=>src.TripId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<Delivery, SearchDeliveryDTO>()
                .ForMember(dest => dest.DriverEmail, opt => opt.MapFrom(src => src.Driver.user.Email))
                .ReverseMap()
                .ForMember(dest => dest.Driver, opt => opt.Ignore());

            CreateMap<Delivery, GetDeliveryByStatus>()
                .ForMember(dest => dest.DeliveryStatus, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.DeliveryId, opt => opt.MapFrom(src => src.Id));



        }
    }
}
