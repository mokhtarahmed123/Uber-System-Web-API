using Uber.Uber.Domain.Entities;

namespace Uber.Uber.Application
{
    public interface ICategoryRepo : CommonWithDatabase<Category>
    {
        Task<Category> GetCategoryWithItems(int id);


    }
}
