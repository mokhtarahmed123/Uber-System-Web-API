using System.ComponentModel.DataAnnotations;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application
{
    public class CreateDeliveryDTO
    {

        [Required(ErrorMessage = "Pickup latitude is required.")]
        [Range(-90, 90, ErrorMessage = "Pickup latitude must be between -90 and 90.")]
        public double PickupLat { get; set; }

        [Required(ErrorMessage = "Pickup longitude is required.")]
        [Range(-180, 180, ErrorMessage = "Pickup longitude must be between -180 and 180.")]
        public double PickupLng { get; set; }

        [Required(ErrorMessage = "Dropoff latitude is required.")]
        [Range(-90, 90, ErrorMessage = "Dropoff latitude must be between -90 and 90.")]
        public double DropoffLat { get; set; }

        [Required(ErrorMessage = "Dropoff longitude is required.")]
        [Range(-180, 180, ErrorMessage = "Dropoff longitude must be between -180 and 180.")]
        public double DropoffLng { get; set; }

        [Required(ErrorMessage = "Enter the delivery status.")]
        [Display(Name = "Delivery Status")]
        public DeliveryStatus Status { get; set; }
        [Required(ErrorMessage = " Enter Driver Email ")]
        [Display(Name = " Driver Email")]
        [DataType(DataType.EmailAddress)]
        public string DriverEmail { get; set; }
        [Required(ErrorMessage = " Enter Trip Id ")]
        [Display(Name = " Trip Id ")]
        [Range(1,maximum:1000000)]
        public int TripId { get; set; }


    }
}
