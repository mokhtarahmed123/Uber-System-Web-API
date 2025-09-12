using Uber.Uber.Application.DTOs.RideRequestDTOs;

namespace Uber.Uber.Application.Interfaces
{
    public interface IRideRequestsService
    {

        Task<CreateRideRequestDTO> CreateRideRequest(CreateRideRequestDTO requestDTO);
        Task <bool> DeleteRequest(int  id);
        Task<UpdateRideRequestDTO> UpdateRideRequest(int id,UpdateRideRequestDTO requestDTO);
        Task<RideRequestDetailsDTO> RideRequestDetails(int id);
        Task<List<RideRequestListDTO>> ListRideRequests();
        Task<List<RideRequestListDTO>> GetPendingRequests();
        Task<List<RideRequestListDTO>> GetAcceptedRequests();
        Task<List<RideRequestListDTO>> SearchRideRequests(SearchRideRequestDTO search);
        Task<List<RideRequestListDTO>> GetCompletedRequests();
        Task<bool> AcceptRideRequest(int id);
        Task<bool> RejectRideRequest(int id);
        Task<bool> CancelRideRequest(int id);
    }
}
