using System.ComponentModel.DataAnnotations;

namespace Uber.Uber.Domain.DTOs
{
    public class SignUpasMerchant:SignUpDTOAsCustomer
    {
        [Required(ErrorMessage = "Please enter the address.")]
        public string Address { get; set; }
        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }
        


    }
}
