using System.ComponentModel.DataAnnotations;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application.DTOs.DriverProfileDTOS
{
    public class ChangeDriverStatusDTO
    {
        [Required]
        public DriverStatus Status { get; set; } 
    }
}
