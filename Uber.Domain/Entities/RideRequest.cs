using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Domain.Entities
{
    public class RideRequest
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey(nameof(Rider))]
        public int RiderID { get; set; }
        public Customer Rider { get; set; }

        [Required]
        public double PickupLat { get; set; }
        [Required]
        public double PickupLng { get; set; }
        [Required]
        public double DestinationLat { get; set; }
        [Required]
        public double DestinationLng { get; set; }

        [Required]
        [Display(Name = "Ride Requests Stataus ")]
        public RideRequestsStatus RideRequestStatus { get; set; }
        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
        



    }
}
