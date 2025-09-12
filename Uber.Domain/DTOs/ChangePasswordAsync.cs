using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Uber.Uber.Domain.DTOs
{
    public class ChangePasswordAsync:LoginDTO
    {
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
