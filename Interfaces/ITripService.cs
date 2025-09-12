using Uber.Uber.Application.DTOs.TripDTOs;

namespace Uber.Uber.Application.Interfaces
{
    public interface ITripService
    {
        Task<CreateTripDTO> CreateTripAsync(CreateTripDTO create);
        Task<UpdateTripDTO> UpdateTripAsync(int id, UpdateTripDTO update);
        Task<UpdateTripStatusDTO> UpdateTripStatusAsync(int id, UpdateTripStatusDTO update);
        Task<bool> DeleteTripAsync(int id);
        Task<TripDetailsDTO> GetTripDetailsAsync(int id);
        Task<List<SearchTripDTO>> SearchTripDTO(SearchTripDTO search);
        Task<List<TripListDTO>> GetAllTripsAsync();
        Task<List<TripListDTO>> GetTripsByDriverAsync(string driverEmail);
    



    }
}
