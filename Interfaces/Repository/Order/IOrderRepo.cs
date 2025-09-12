using Uber.Uber.Domain.Entities;

namespace Uber.Uber
{
    public interface IOrderRepo : CommonWithDatabase<Order>
    {
        Task<List<Order>> GetOrdersByCustomerEmailAsync(string email);
        Task<List<Order>> GetOrdersByMerchantEmailAsync(string email);
    }
}
