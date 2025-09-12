using System.ComponentModel.DataAnnotations;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application.DTOs.TripDTOs
{
    public class UpdateTripDTO
    {   
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Range(0.1, 10000)]
        public double DistanceKm { get; set; }

        [Range(1, 1440)]
        public double DurationMin { get; set; }

        [Range(1, 100000)]
        public double TotalCost { get; set; }

        [Required]
        public StatausTrip StatausTrip { get; set; }

        [EmailAddress]
        public string DriverEmail { get; set; }
    }
}
