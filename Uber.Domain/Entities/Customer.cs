using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uber.Uber.Domain.Entities
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        [Required]
        [ForeignKey("UserApp")]
        public string AppUserId { get; set; }
        public virtual User UserApp { get; set; }
        [Required]
        [InverseProperty("FromUser")]
        public ICollection<Complaints> Complaints { get; set; } = new List<Complaints>();
        [InverseProperty("customer")]

        public ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();
        public ICollection<Order> Order { get; set; } = new List<Order>();
        public ICollection<RideRequest> RideRequest { get; set; } = new List<RideRequest>();
        public ICollection<Payment> Payment { get; set; } = new List<Payment>();
        public ICollection<Trip> trip { get; set; } = new List<Trip>();



    }
}
