using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Uber.Uber.Application.DTOs.CategoryDTO;
using Uber.Uber.Domain.Entities;

namespace Uber.Uber.Application
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly UberContext context;
        private readonly ILogger<Category> logger;
        private readonly IMapper mapper;

        public CategoryRepo(UberContext context, ILogger<Category> logger, IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }
        public async Task Create(Category entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                throw new ArgumentException("Category Name cannot be empty");
            }
            await context.Categories.AddAsync(entity);
            await SaveChange();
            logger.LogInformation($"Category created successfully '{entity.Name}'");

        }

        public async Task Delete(int id)
        {
            var IsFound = await context.Categories.FindAsync(id);
            if (IsFound != null)
            {

                context.Categories.Remove(IsFound);
                await SaveChange();
                logger.LogInformation("User with ID {Id} deleted successfully.", id);
                return;
            }
            logger.LogWarning("User with ID {Id} not found for deletion.", id);
            throw new NotFoundException($"Category with ID {id} not found.");
        }

        public async Task<List<Category>> FindAll(int page = 1, int pageSize = 20)
        {
            return await context.Categories
          .Skip((page - 1) * pageSize)
           .Take(pageSize)
          .ToListAsync();
        }

        public async Task<Category> GetByID(int ID)
        {
            var IsFound = await context.Categories.FindAsync(ID);
            if (IsFound != null)
            {
                return IsFound;
            }
            logger.LogWarning($"Category with ID {ID} not found.");
            throw new NotFoundException("Category Is Not Found , Try Another");
        }
        public async Task<Category> Update(int ID, Category entity)
        {
            var CAT = await context.Categories.FindAsync(ID);
            if (CAT != null)
            {
                if (string.IsNullOrWhiteSpace(entity.Name))
                    throw new ArgumentException("User Name cannot be empty");
                CAT.Name = entity.Name;
                await SaveChange();
                logger.LogInformation(" Category  with ID {Id} updated successfully.", ID);
                return CAT;

            }
            logger.LogWarning("Category with ID {Id} not found for update.", ID);
            throw new NotFoundException($"Category with ID {ID} not found.");
        }
        public async Task<Category> GetCategoryWithItems(int id)
        {
            var Check = await context.Categories.Include(a => a.Items).FirstOrDefaultAsync(i => i.Id == id);
            if (Check != null)
            {
                return Check;
            }
            logger.LogWarning($"Category with ID {id} not found.");
            throw new NotFoundException($"Category Is With {id}  Not Found , Try Another");
        }
        public async Task SaveChange()
        {

            await context.SaveChangesAsync();
        }




    }
}
