using System.ComponentModel.DataAnnotations;

namespace Uber.Uber.Domain.DTOs
{
    public class UpdateUserDTO
    {
        [Required(ErrorMessage = " Please Enter Your Email Address  ")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } 
        
        [Required(ErrorMessage = " Please Enter Your Name   ")]
        public string Name { get; set; }  
        [Required(ErrorMessage = " Please Enter Your Phone  ")]
        [DataType(DataType.PhoneNumber)]

        public string Phone { get; set; }


    }
}
