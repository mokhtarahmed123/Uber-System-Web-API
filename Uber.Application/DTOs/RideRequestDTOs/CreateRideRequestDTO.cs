using System.ComponentModel.DataAnnotations;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application.DTOs.RideRequestDTOs
{
    public class CreateRideRequestDTO
    {
        [Required]
        public string RiderEmail { get; set; }

        [Required]
        public double PickupLat { get; set; }

        [Required]
        public double PickupLng { get; set; }

        [Required]
        public double DestinationLat { get; set; }

        [Required]
        public double DestinationLng { get; set; }
        [Required]
        [Display(Name = "Ride Requests Status ")]
        public RideRequestsStatus RideRequestStatus { get; set; } = RideRequestsStatus.Pending;
    }
}
