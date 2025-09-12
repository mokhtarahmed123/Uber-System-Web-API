using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application.DTOs.DriverProfileDTOS
{
    public class GetAllDriversDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email {  get; set; }
        public string VehicleType { get; set; }
        public string PlateNumber { get; set; }
        public double AverageRating { get; set; }
        public int NumberOfTrips { get; set; }
        public string Status { get; set; }

    }
}
