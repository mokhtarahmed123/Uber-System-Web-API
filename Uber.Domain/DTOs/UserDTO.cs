using System.ComponentModel.DataAnnotations;

namespace Uber.Uber.Domain.DTOs
{
    public class UserDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = " Please Enter Your Phone  ")]
        [DataType(DataType.PhoneNumber)]

        public string Phone { get; set; }
    }
}
