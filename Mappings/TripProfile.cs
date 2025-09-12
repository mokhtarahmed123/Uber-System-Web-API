using AutoMapper;
using Uber.Uber.Application.DTOs.TripDTOs;

namespace Uber.Uber.Application.Mappings
{
    public class TripProfile:Profile
    {
        public TripProfile()
        {

            CreateMap<CreateTripDTO, Trip>()
                       .ForMember(dest => dest.DriverId, opt => opt.Ignore()) // هيتجاب من السيرفيس بالـ Email
                       .ForMember(dest => dest.customerId, opt => opt.Ignore()) // هيتجاب من السيرفيس بالـ Email
                       .ForMember(dest => dest.CarImagePath, opt => opt.MapFrom(src => src.CarImagePath));

            // Entity → DTO
            CreateMap<Trip, CreateTripDTO>()
                .ForMember(dest => dest.DriverEmail, opt => opt.MapFrom(src => src.Driver.user.Email))
                .ForMember(dest => dest.RiderEmail, opt => opt.MapFrom(src => src.customer.UserApp.Email))
                .ForMember(dest => dest.CarImagePath, opt => opt.MapFrom(src => src.CarImagePath))
                .ForMember(dest => dest.RideRequestId, opt => opt.MapFrom(src => src.RideRequestId))
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderID));


            CreateMap<UpdateTripDTO, Trip>()
              .ForMember(dest => dest.DriverId, opt => opt.Ignore()) 
              .ForMember(dest => dest.customerId, opt => opt.Ignore());

            CreateMap<Trip, UpdateTripDTO>()
                .ForMember(dest => dest.DriverEmail, opt => opt.MapFrom(src => src.Driver.user.Email));

            CreateMap<UpdateTripStatusDTO, Trip>()
                .ForMember(dest => dest.StatausTrip, opt => opt.MapFrom(src => src.StatausTrip));

    
            CreateMap<Trip, TripDetailsDTO>()
                .ForMember(dest => dest.DriverEmail, opt => opt.MapFrom(src => src.Driver.user.Email))
                .ForMember(a=>a.RiderEmail , opt=>opt.MapFrom(a=>a.customer.UserApp.Email))
                .ForMember(dest => dest.CarImagePath, opt => opt.MapFrom(src => src.CarImagePath))
                .ForMember(a=>a.OrderId , opt=>opt.MapFrom(a=>a.OrderID));

            CreateMap<Trip, TripListDTO>()
                .ForMember(dest => dest.DriverEmail, opt => opt.MapFrom(src => src.Driver.user.Email))
                .ForMember(dest=>dest.RiderEmail , opt=>opt.MapFrom(opt=>opt.customer.UserApp.Email))
                .ForMember(dest=>dest.OrderID , opt=>opt.MapFrom(src=>src.OrderID));
            CreateMap<Trip, SearchTripDTO>()
     .ForMember(dest => dest.DriverEmail,
         opt => opt.MapFrom(src => src.Driver != null && src.Driver.user != null ? src.Driver.user.Email : null))
     .ForMember(dest => dest.RiderEmail,
         opt => opt.MapFrom(src => src.customer != null ? src.customer.UserApp.Email : null))
     .ForMember(dest => dest.StatausTrip,
         opt => opt.MapFrom(src => src.StatausTrip))  ;

        }
    }
}
