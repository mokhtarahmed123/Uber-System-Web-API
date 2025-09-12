using Uber.Uber.Application.DTOs.CategoryDTO;
using Uber.Uber.Domain.Entities;

namespace Uber.Uber
{
    public interface ICategoryService
    {
        // Services Implement it  
        Task<CreateOrUpdateCategoryDTO> CreateCategory(CreateOrUpdateCategoryDTO category);
        Task<bool> DeleteCategory(int id);
        Task<CreateOrUpdateCategoryDTO> UpdateCategory(int id, CreateOrUpdateCategoryDTO category);
        Task<CreateOrUpdateCategoryDTO> GetCategoryById(int id);
        Task<List<Category>> GetAllCategories();
        Task<GetCategoryWithItems> GetCategoryWithItems(int id);


    }
}
