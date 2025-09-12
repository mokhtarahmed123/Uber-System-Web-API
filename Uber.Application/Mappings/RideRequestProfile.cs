using AutoMapper;
using Uber.Uber.Application.DTOs.RideRequestDTOs;
using Uber.Uber.Domain.Entities;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application.Mappings
{
    public class RideRequestProfile : Profile
    {
        public RideRequestProfile()
        {


            CreateMap<CreateRideRequestDTO, RideRequest>()

                .ForMember(dest => dest.DestinationLat, opt => opt.MapFrom(src => src.DestinationLat))
                .ForMember(dest => dest.DestinationLng, opt => opt.MapFrom(src => src.DestinationLng))
                .ForMember(dest => dest.PickupLat, opt => opt.MapFrom(src => src.PickupLat))
                .ForMember(dest => dest.PickupLng, opt => opt.MapFrom(src => src.PickupLng))
                .ForMember(dest => dest.RideRequestStatus, opt => opt.MapFrom(src => RideRequestsStatus.Pending))
                    .ForMember(dest => dest.RiderID, opt => opt.Ignore())
               .ReverseMap()
                ;

            // Update RideRequest
            CreateMap<UpdateRideRequestDTO, RideRequest>()
             .ForMember(dest => dest.DestinationLat, opt => opt.MapFrom(src => src.DestinationLat))
             .ForMember(dest => dest.DestinationLng, opt => opt.MapFrom(src => src.DestinationLng))
             .ForMember(dest => dest.RideRequestStatus,
                 opt => opt.MapFrom(src =>
                     !string.IsNullOrEmpty(src.Status)
                         ? Enum.Parse<RideRequestsStatus>(src.Status, true)
                         : RideRequestsStatus.Pending))
             .ReverseMap();

            CreateMap<RideRequest, RideRequestListDTO>()
                .ForMember(dest => dest.RideRequestStatus, opt => opt.MapFrom(src => src.RideRequestStatus.ToString()))
                .ForMember(dest => dest.PickupLat, opt => opt.MapFrom(src => src.PickupLat))
                .ForMember(dest => dest.PickupLng, opt => opt.MapFrom(src => src.PickupLng))
                .ForMember(dest => dest.DestinationLat, opt => opt.MapFrom(src => src.DestinationLat))
                .ForMember(dest => dest.DestinationLng, opt => opt.MapFrom(src => src.DestinationLng))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest=>dest.RiderEmail , opt=>opt.MapFrom(src=>src.Rider.UserApp.Email) );

            // Details RideRequest
            CreateMap<RideRequest, RideRequestDetailsDTO>()
                .ForMember(dest => dest.PickupLat, opt => opt.MapFrom(src => src.PickupLat))
                .ForMember(dest => dest.PickupLng, opt => opt.MapFrom(src => src.PickupLng))
                .ForMember(dest => dest.DestinationLat, opt => opt.MapFrom(src => src.DestinationLat))
                .ForMember(dest => dest.DestinationLng, opt => opt.MapFrom(src => src.DestinationLng))
                .ForMember(dest => dest.RideRequestStatus, opt => opt.MapFrom(src => src.RideRequestStatus.ToString()))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.RiderEmail, opt => opt.MapFrom(src => src.Rider.UserApp.Email));








        }
    }
}
