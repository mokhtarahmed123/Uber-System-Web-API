using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Domain.Entities
{
    public class Reviews
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey(nameof(Trip))]
        public int TripID { get; set; }
        public Trip Trip { get; set; }

        public string Massege { get; set; }

        [ForeignKey(nameof(customer))]
        public int customerID { get; set; }
        public Customer customer { get; set; }

        [ForeignKey(nameof(Driver))]
        public int DriverID { get; set; }
        public DriverProfile Driver { get; set; }

        [Required]
        public Rating Rating { get; set; }


    }
}
