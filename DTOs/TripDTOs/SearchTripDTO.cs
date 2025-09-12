using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application.DTOs.TripDTOs
{
    public class SearchTripDTO
    {
        public string? DriverEmail { get; set; }
        public string ? RiderEmail {  get; set; }
        public StatausTrip? StatausTrip { get; set; }
    }
}
