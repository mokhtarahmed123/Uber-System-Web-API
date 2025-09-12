using System.ComponentModel.DataAnnotations;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application.DTOs.ReviewsDTOs
{
    public class UpdateReviewDTO
    {
        [Required]
        public Rating Rating { get; set; }

        [StringLength(500)]
        public string Message { get; set; }
    }
}
