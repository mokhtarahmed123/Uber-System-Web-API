using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Domain.DTOs
{
    public class SignUpDTOAsDriver: SignUpDTOAsCustomer
    {
        [Required(ErrorMessage = "Please enter the type of vehicle")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Vehicle type must be between 3 and 50 characters")]
        public string VehicleType { get; set; }

        [Required(ErrorMessage = "Please enter the plate number")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Plate number must be between 5 and 20 characters")]
        public string PlateNumber { get; set; }

        [Required(ErrorMessage = "Please upload an image of the license")]
        [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }

        //public string? LicenseImagePath { get; set; }
    
        public DriverStatus Status { get; set; }
    }
}
