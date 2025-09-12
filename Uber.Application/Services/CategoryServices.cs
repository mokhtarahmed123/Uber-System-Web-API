using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Uber.Uber.Application;
using Uber.Uber.Application.DTOs.CategoryDTO;

using Uber.Uber.Domain.Entities;

namespace Uber.Uber
{
    public class CategoryServices : ICategoryService
    {
        private readonly UberContext context;
        private readonly ICategoryRepo _category;
        public readonly IMapper Mapper;
        public readonly ILogger<Category> Logger;

        public CategoryServices(UberContext context, ICategoryRepo category, IMapper mapper, ILogger<Category> logger)
        {
            this.context = context;
            this._category = category;
            Mapper = mapper;
            Logger = logger;
        }


        public async Task<CreateOrUpdateCategoryDTO> CreateCategory(CreateOrUpdateCategoryDTO dto)
        {
            if (dto == null) 
                 throw new ArgumentNullException(nameof(dto));
            

            var categoryEntity = Mapper.Map<Category>(dto);

            await _category.Create(categoryEntity);

            return Mapper.Map<CreateOrUpdateCategoryDTO>(categoryEntity);
        }

        public async Task<bool> DeleteCategory(int id)
        {
            await _category.Delete(id);
            return true;

        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await _category.FindAll();

        }

        public async Task<CreateOrUpdateCategoryDTO> GetCategoryById(int id)
        {
            var Category = await _category.GetByID(id);
            return Mapper.Map<CreateOrUpdateCategoryDTO>(Category);
        }

        public async Task<GetCategoryWithItems> GetCategoryWithItems(int id)
        {
            var category = await _category.GetCategoryWithItems(id);

            if (category == null)
            {
                Logger.LogWarning("Category with ID {Id} not found.", id);
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            var Q = await context.Items.Include(a=>a.category).FirstOrDefaultAsync(i=>i.Id == id);

            
            var items = await context.Items
                                     .Where(i => i.CategoryId == id)
                                     .ToListAsync();
            return new GetCategoryWithItems
            {
                CategoryName = category.Name,
                ItemsName = string.Join(", ", items.Select(i => i.Name)),
                ItemsNumber = items.Count,
                Quanitity = items.Sum(i => i.Quantity)

            };


        }

        public async Task<CreateOrUpdateCategoryDTO> UpdateCategory(int id, CreateOrUpdateCategoryDTO category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));
            var _category3 = Mapper.Map<Category>(category);
            var updatedCategory = await _category.Update(id, _category3);
            return Mapper.Map<CreateOrUpdateCategoryDTO>(updatedCategory);
        }
    }
}
