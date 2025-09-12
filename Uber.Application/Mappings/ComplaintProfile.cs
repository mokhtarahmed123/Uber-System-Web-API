using AutoMapper;
using Uber.Uber.Application.DTOs.ComplaintsDTOs;

namespace Uber.Uber.Application.Mappings
{
    public class ComplaintProfile:Profile
    {
        public ComplaintProfile()
        {
            CreateMap<Complaints, CreateComplaintsdto>()
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.FromUser.UserApp.Email))
                .ForMember(dest => dest.DriverEmail, opt => opt.MapFrom(src => src.Driver.user.Email))
                .ForMember(dest => dest.TripId, opt => opt.MapFrom(src => src.Trip.ID))
                .ForMember(dest=>dest.Massege , opt=>opt.MapFrom(src=>src.Message))
                .ReverseMap()
                .ForMember(dest => dest.FromUser, opt => opt.Ignore())
                .ForMember(dest => dest.Driver, opt => opt.Ignore())
                .ForMember(dest => dest.Trip, opt => opt.Ignore());

            CreateMap<Complaints, UpdateComplaintsdto>()
                .ReverseMap()
                .ForMember(dest => dest.FromUser, opt => opt.Ignore())
                .ForMember(dest => dest.Driver, opt => opt.Ignore())
                .ForMember(dest => dest.Trip, opt => opt.Ignore());
            CreateMap<Complaints, ComplaintDetailsDTO>()
          .ForMember(dest => dest.FromUserName, opt => opt.MapFrom(src => src.FromUser.UserApp.Email))  // Assuming User has FullName
          .ForMember(dest => dest.FromUserID, opt => opt.MapFrom(src => src.FromUserID))
          .ForMember(dest => dest.AgainstUserName, opt => opt.MapFrom(src => src.Driver.user.Name))
          .ForMember(dest => dest.AgainstUserId, opt => opt.MapFrom(src => src.AgainstUserId))
          .ForMember(dest => dest.TripID, opt => opt.MapFrom(src => src.TripID));

            CreateMap<Complaints, ListComplaintsDTO>()
                .ForMember(dest => dest.FromUserID, opt => opt.MapFrom(src => src.FromUserID))
                .ForMember(dest => dest.AgainstUserId, opt => opt.MapFrom(src => src.AgainstUserId))
                .ForMember(dest => dest.TripID, opt => opt.MapFrom(src => src.TripID));

            CreateMap<Complaints, SearchComplaintDTO>()
                .ReverseMap()
                .ForMember(dest => dest.FromUser, opt => opt.Ignore())
                .ForMember(dest => dest.Driver, opt => opt.Ignore())
                .ForMember(dest => dest.Trip, opt => opt.Ignore());

            CreateMap<Complaints, ResolveComplaintDTO>()
                .ReverseMap()
                .ForMember(dest => dest.FromUser, opt => opt.Ignore())
                .ForMember(dest => dest.Driver, opt => opt.Ignore())
                .ForMember(dest => dest.Trip, opt => opt.Ignore());

        }
    }
}
