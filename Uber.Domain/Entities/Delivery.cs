using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Domain.Entities
{
    public class Delivery

    {
        [Key]
        public int Id { get; set; }
        [Required]
        public double PickupLat { get; set; }
        [Required]
        public double PickupLng { get; set; }
        [Required]
        public double DropoffLat { get; set; }
        [Required]
        public double DropoffLng { get; set; }
        [Required(ErrorMessage = " Enter The Sataus ")]
        [Display(Name = " Delivery Sataus ")]
        public DeliveryStatus Status { get; set; }

        [ForeignKey("Driver")]
        public int DriverID { get; set; }
        public DriverProfile Driver { get; set; }

        [ForeignKey(nameof(trip))]
        public int TripId {  get; set; }
        public Trip trip { get; set; }
    }
}
