using AutoMapper;
using Uber.Uber.Application.DTOs;
using Uber.Uber.Application.DTOs.DriverProfileDTOS;

namespace Uber.Uber.Application
{
    public class DriverProfileProfile : Profile
    {
        public DriverProfileProfile()
        {
            CreateMap<CreateDriverProfile, DriverProfile>()
                .ForMember(dest => dest.DriverID, opt => opt.MapFrom(src => src.DriverEmail))
                .ForMember(dest => dest.LicenseImagePath, opt => opt.MapFrom(src => src.LicenseImagePath))
                .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType))
                .ForMember(dest => dest.PlateNumber, opt => opt.MapFrom(src => src.PlateNumber))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.DriverStatus))
                .ForMember(dest => dest.Image, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<DriverProfile, GetDriverProfilesDetails>()
          .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.user.Name))
          .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.user.Email))
          .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType))
          .ForMember(dest => dest.PlateNumber, opt => opt.MapFrom(src => src.PlateNumber))
          .ForMember(dest => dest.LicenseImagePath, opt => opt.MapFrom(src => src.LicenseImagePath))
          .ForMember(dest => dest.NumberOfTrips, opt => opt.MapFrom(src => src.Trips.Count));

            CreateMap<UpdateDriverProfile, DriverProfile>()
    .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType))
    .ForMember(dest => dest.PlateNumber, opt => opt.MapFrom(src => src.PlateNumber))
    .ForMember(dest => dest.LicenseImagePath, opt => opt.MapFrom(src => src.LicenseImagePath))
    .ForMember(dest => dest.Image, opt => opt.Ignore())
    .ReverseMap();
            CreateMap<DriverProfile, GetAllDriversDTO>()
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.user.Name))
        .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.user.Email))
        .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType))
        .ForMember(dest => dest.PlateNumber, opt => opt.MapFrom(src => src.PlateNumber))
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
        .ForMember(dest => dest.NumberOfTrips, opt => opt.MapFrom(src => src.Trips.Count));
            CreateMap<ChangeDriverStatusDTO, DriverProfile>()
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
        .ReverseMap();
        }
    }
}
