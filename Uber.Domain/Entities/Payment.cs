using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Uber.Uber.Domain.Entities;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber
{
    public class Payment
    {
        Trip tr = new Trip();
        [Key]
        public int ID { get; set; }
        [ForeignKey("Trip_Id")]
        public int TripID { get; set; }
        public Trip Trip_Id { get; set; }
        [Required]
        public double TotalPrice { get; set; }
        [Required]
        public PaymentMethodEnum Method { get; set; }
        [Required]
        public PaymentStatus PaymentStatus { get; set; }
        [ForeignKey("customer")]
        public int customerid { get; set; }
        public Customer customer { get; set; }
        [ForeignKey("Merchant")]
        public int? Merchantid { get; set; }
        public Merchant? Merchant { get; set; }

    }
}
