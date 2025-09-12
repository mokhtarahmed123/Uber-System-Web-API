using System.ComponentModel.DataAnnotations;

namespace Uber.Uber.Domain.DTOs
{
    public class ForgetPasswordDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

    }
}
