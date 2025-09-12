using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uber.Uber.Domain.Entities
{
    public class User : IdentityUser
    {
        #region Attributes
        [Required(ErrorMessage = " \'Please Enter Name\' ")]
        [Display(Name = "Full Name")]
        [MaxLength(50, ErrorMessage = " \'Name Must By Less Than 50\'")]
        [MinLength(3, ErrorMessage = " \'Name Must Be Greater Than 3\'")]
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
        [Required]
        [ForeignKey(nameof(Role))]
        public string RoleId { get; set; }
        public RoleApp Role { get; set; }

        #endregion
        ///Lists
        public virtual ICollection<Complaints> ComplaintsFromUser { get; set; }
        public virtual ICollection<Complaints> ComplaintsAgainstUser { get; set; }

        public virtual ICollection<Delivery> Delivery { get; set; } = new List<Delivery>();
        public virtual ICollection<Order> Order { get; set; } = new List<Order>();
        public virtual ICollection<Reviews> ReviewsFromUser { get; set; }
        public virtual ICollection<Reviews> ReviewsToUser { get; set; }
        public virtual ICollection<RideRequest> RideRequests { get; set; } = new List<RideRequest>();
        public ICollection<Trip> TripsAsDriver { get; set; } = new List<Trip>();
        public ICollection<Trip> TripsAsRider { get; set; } = new List<Trip>();

        public virtual ICollection<DriverProfile> DriverProfile { get; set; } = new List<DriverProfile>();

        public ICollection<IdentityUserRole<string>> UserRoles { get; set; }







    }
}
