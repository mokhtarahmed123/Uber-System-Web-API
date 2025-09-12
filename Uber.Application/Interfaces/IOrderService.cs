using Uber.Uber.Application.DTOs.OrderDTO;

namespace Uber.Uber.Application
{
    public interface IOrderService
    {
        Task<CreateOrUpdateOrder> CreateOrder(CreateOrUpdateOrder Create);
        Task<CreateOrUpdateOrder> UpdateOrder(int id, CreateOrUpdateOrder Create);
        Task<bool> DeleteOrder(int id);
        Task<List<OrderListDTO>> SearchOrdersAsync(SearchOrderDTO search);
        Task<OrderDetailsDTO> GetOrderByIdAsync(int orderId);
        Task<List<OrderListDTO>> GetAllOrdersAsync();
        Task<List<OrderByCustomerDTO>> GetOrdersByCustomerEmailAsync(string customerEmail);
        Task<List<OrderListDTO>> GetOrdersByMerchantEmailAsync(string merchantEmail);
        Task<int> GetCustomerOrderCountAsync(string customerEmail);




    }
}
