using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application.DTOs.TripDTOs
{
    public class TripListDTO
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public double TotalCost { get; set; }
        public StatausTrip StatausTrip { get; set; }
        public string DriverEmail { get; set; }
        public string RiderEmail { get; set; }
        public int OrderID {  get; set; }
    }
}
