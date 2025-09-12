namespace Uber.Uber.Application.DTOs.RideRequestDTOs
{
    public class SearchRideRequestDTO
    {
        public string? RiderEmail { get; set; }
        public string? Status { get; set; } // Pending, Accepted, Completed
    }
}
