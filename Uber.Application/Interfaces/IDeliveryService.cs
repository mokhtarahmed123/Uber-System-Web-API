using Uber.Uber.Application.DTOs.DeliveryDTOs;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application.Interfaces
{
    public interface IDeliveryService
    {
        Task<CreateDeliveryDTO> CreateDeliveryDTO(CreateDeliveryDTO deliveryDTO);
        Task<UpdateDeliveryDTO> UpdateDeliveryDTO(int id, UpdateDeliveryDTO deliveryDTO);
        Task<bool> DeleteDelivery(int id);
        Task<List<ListDeliveryDTO>> ListDeliveryDTO();
        Task<DeliveryDetailsDTO> GetDeliveryByIdAsync(int id);
        Task<List<ListDeliveryDTO>> SearchDeliveriesAsync(SearchDeliveryDTO searchDTO);

        Task<List<ListDeliveryDTO>> GetDeliveriesByStatusAsync(DeliveryStatus status);

    }
}
