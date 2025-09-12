using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Uber.Uber.Domain.Entities;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber
{
    public class Trip
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; }
        [Required]
        public double DistanceKm { get; set; }
        [Required]
        [DataType(DataType.Duration)]
        public double DurationMin { get; set; }
        [Required]
        public double TotalCost { get; set; }

        [Required]
        public StatausTrip StatausTrip { get; set; }

        [Required(ErrorMessage = "Please upload an image")]

        public string? CarImagePath { get; set; }

        [ForeignKey(nameof(Driver))]
        public int DriverId { get; set; }
        public DriverProfile Driver { get; set; }

        [ForeignKey(nameof(customer))]
        public int customerId { get; set; }
        public Customer customer { get; set; }

        [ForeignKey(nameof(RideRequest))]
        public int? RideRequestId { get; set; }
        public RideRequest? RideRequest { get; set; }

        [ForeignKey(nameof(order))]

        public int? OrderID { get; set; }
        public Order? order { get; set; }
            


        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<Complaints> Complaints { get; set; } = new List<Complaints>();
        public virtual ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();
      
 
    }
}
