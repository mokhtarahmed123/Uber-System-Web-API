using AutoMapper;
using Uber.Uber.Application.DTOs.CategoryDTO;
using Uber.Uber.Domain.Entities;

namespace Uber.Uber
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {

            // DTO => Entity && Entity => DTO
            CreateMap<CreateOrUpdateCategoryDTO, Category>().ReverseMap();


        }
    }
}
