using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Uber.Uber.Application.Validations;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Domain.DTOs
{
    public class SignUpDTOAsCustomer
    {
        [Required(ErrorMessage =" Please Enter Your Name ")]
        [MinLength(3,ErrorMessage =" Name Must Be Greater Than 3  ")]
        [MaxLength(70,ErrorMessage =" Name Must Be Less Than 70 ")]
        public string Name { get; set; }
        [Required(ErrorMessage = " Please Enter Your Phone Number ")]
        [DataType(DataType.PhoneNumber)]
        [UniquePhoneNumber(ErrorMessage =" This Phone Is Found , Try Another ")]
        public string Phone {  get; set; }
        [Required(ErrorMessage =" Please Enter Your Email Address  ")]
        [DataType(DataType.EmailAddress)]
        [UniqueEmailUser(ErrorMessage = " This Email Is Found , Try Another ")]

        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        [DataType(DataType.Password)]

        public string Password { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }





    }
}
