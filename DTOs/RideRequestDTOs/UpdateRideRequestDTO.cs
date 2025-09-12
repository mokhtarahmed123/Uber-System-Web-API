namespace Uber.Uber.Application.DTOs.RideRequestDTOs
{
    public class UpdateRideRequestDTO
    {
        public double? DestinationLat { get; set; }
        public double? DestinationLng { get; set; }
        public string? RiderEmail { get; set; }
        public string? Status { get; set; }
    }
}
