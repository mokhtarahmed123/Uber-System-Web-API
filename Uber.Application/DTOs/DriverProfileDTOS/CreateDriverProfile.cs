using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Uber.Uber.Application.Validations;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application.DTOs.DriverProfileDTOS
{
    public class CreateDriverProfile
    {
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
        [DataType(DataType.Upload)]

        public string? LicenseImagePath { get; set; }
        [Required(ErrorMessage = "Please Enter Statue Of Driver ")]

        public DriverStatus DriverStatus { get; set; } = DriverStatus.Active;
        [UniqueEmailUser]
        [EmailAddress]
        [Required(ErrorMessage =" Enter Email Of Driver ")]
        public string DriverEmail{ get; set; }

    }
}
