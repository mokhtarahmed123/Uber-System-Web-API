using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Uber.Uber.Domain.Entities;

namespace Uber.Uber
{
    public class Complaints
    {
        [Key]
        public int Id { get; set; }

        public string Message { get; set; }


        [ForeignKey(nameof(FromUser))]
        public int FromUserID { get; set; }
        public Customer FromUser { get; set; }

  
        [ForeignKey(nameof(Driver))]
        public int AgainstUserId { get; set; }
        public DriverProfile Driver { get; set; }

        [ForeignKey(nameof(Trip))]
        public int TripID { get; set; }
        public Trip Trip { get; set; }

        public bool IsResolved { get; set; } = false;

    }
}
