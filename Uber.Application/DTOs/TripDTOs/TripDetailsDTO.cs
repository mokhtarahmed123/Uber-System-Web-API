using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application.DTOs.TripDTOs
{
    public class TripDetailsDTO
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double DistanceKm { get; set; }
        public double DurationMin { get; set; }
        public double TotalCost { get; set; }
        public StatausTrip StatausTrip { get; set; }
        public string CarImagePath { get; set; }
        public string DriverEmail { get; set; }
        public int RideRequestId { get; set; }
        public string RiderEmail { get; set; }
        public RideRequestsStatus RideRequestsStatus { get; set; }
        public int OrderId {  get; set; }
    }
}
