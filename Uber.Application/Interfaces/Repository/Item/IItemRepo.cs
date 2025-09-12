using Uber.Uber.Domain.Entities;

namespace Uber.Uber.Application
{
    public interface IItemRepo : CommonWithDatabase<Item>
    {
        Task<List<Item>> GetItemsByCategory(string categoryName);
        Task<List<Item>> GetItemsByMerchant(string merchantEmail);

    }
}
