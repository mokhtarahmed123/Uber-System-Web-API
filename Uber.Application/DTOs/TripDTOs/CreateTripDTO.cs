using System.ComponentModel.DataAnnotations;
using Uber.Uber.Application.Validations;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application
{
    public class CreateTripDTO
    {

        [Required(ErrorMessage = "Start time is required.")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        [DateGreaterThan("StartTime", ErrorMessage = "End time must be after start time.")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "Distance is required.")]
        [Range(0.1, 10000, ErrorMessage = "Distance must be greater than 0 km.")]
        public double DistanceKm { get; set; }

        [Required(ErrorMessage = "Duration is required.")]
        [Range(1, 1440, ErrorMessage = "Duration must be between 1 and 1440 minutes.")]
        public double DurationMin { get; set; }

        [Required(ErrorMessage = "Total cost is required.")]
        [Range(1, 100000, ErrorMessage = "Total cost must be greater than 0.")]
        public double TotalCost { get; set; }

        [Required(ErrorMessage = "Trip status is required.")]
        public StatausTrip StatausTrip { get; set; }

        [Required(ErrorMessage = "Please upload a car image.")]
        [DataType(DataType.Upload)]
        public string CarImagePath { get; set; }

        [Required(ErrorMessage = "Driver email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email.")]
        public string DriverEmail { get; set; }  
        
        [Required(ErrorMessage = "Rider email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email.")]
        public string RiderEmail { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Ride request ID must be greater than 0.")]
        public int? RideRequestId { get; set; }

        [Required(ErrorMessage = "Ride request status is required.")]
        public RideRequestsStatus RideRequestsStatus { get; set; }

        public int? OrderId { get; set; }
    }
}
