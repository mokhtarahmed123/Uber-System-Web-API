//using AutoMapper;
//using Uber.Uber.Domain.DTOs;
//using Uber.Uber.Domain.Entities;

//namespace Uber.Uber.Application.Mappings
//{
//    public class UserProfile :Profile
//    {
//        public UserProfile()
//        {
//            CreateMap<User, UserDTO>()
//                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
//                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
//                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
//                .ForAllOtherMembers(opt => opt.Ignore());

//            CreateMap<UpdateUserDTO, User>()
//                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
//                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
//                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
//                .ForAllOtherMembers(opt => opt.Ignore());


//        }


//    }
//}
