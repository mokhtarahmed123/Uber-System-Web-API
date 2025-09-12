using Uber.Uber.Application.DTOs.ReviewsDTOs;

namespace Uber.Uber.Application.Interfaces
{
    public interface IReviewsService
    {
        Task<ReviewDetailsDTO> CreateReviewAsync(CreateReviewDTO createReviewDTO);
        Task<UpdateReviewDTO> UpdateReviewAsync(int id, UpdateReviewDTO updateReviewDTO);
        Task<bool> DeleteReviewAsync(int id);
        Task<List<ReviewDetailsDTO>> GetCustomerReviewsAsync(string Email);
        Task<List<ReviewDetailsDTO>> DriverReviewsAsync(string Email);
        Task<ReviewDetailsDTO> GetByIdAsync(int id);
        Task<double> GetDriverAverageRatingAsync(string driverEmail);
        Task<List<ReviewDetailsDTO>> GetAll();
        Task<int> GetCustomerReviewsCountAsync(string customerEmail);
        Task<int> GetDriverReviewsCountAsync(string driverEmail);


    }
}
