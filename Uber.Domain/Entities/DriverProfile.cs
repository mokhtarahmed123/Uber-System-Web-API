using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Uber.Uber.Domain.Entities;
using Uber.Uber.Domain.Entities.Enums;


namespace Uber.Uber
{
    public class DriverProfile
    {

        #region Attributes
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = " Please Enter The Type Of  Vehicle")]
        [Display(Name = " Vehicle Type")]
        [MinLength(3, ErrorMessage = " Type Of  Vehicle Must By Greater Than 3")]
        public string VehicleType { get; set; }

        [Required(ErrorMessage = " Please Enter The Number Of  Plate")]
        [Display(Name = " Plate  Number")]
        [MinLength(5, ErrorMessage = "Number Of Plate Must bE Greater Than 5")]
        public string PlateNumber { get; set; }

        [Required(ErrorMessage = "Please upload an image of the license")]
        [Display(Name = "Upload License Image")]
        [NotMapped]
        public IFormFile Image { get; set; }

        public string? LicenseImagePath { get; set; }

        [ForeignKey("user")]
        public string DriverID { get; set; }
        public User user { get; set; }
        #endregion
        public List<Delivery> Deliveries { get; set; } = new List<Delivery>();
        public List<Reviews> reviews { get; set; } = new List<Reviews>();
        public DriverStatus Status { get; set; }
        public List<Trip> Trips { get; set; } = new List<Trip>(); 
        [InverseProperty("Driver")]
        public ICollection<Complaints> ComplaintsReceived { get; set; } = new List<Complaints>();

    }
}
