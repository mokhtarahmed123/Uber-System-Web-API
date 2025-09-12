using System.ComponentModel.DataAnnotations;

namespace Uber.Uber.Domain
{
    public class LoginDTO
    {
        [Required(ErrorMessage = " Please Enter Your Email Address  ")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }    
    }
}
