using System.ComponentModel.DataAnnotations;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application.DTOs.ReviewsDTOs
{
    public class CreateReviewDTO
    {
        [Required]
        public int TripID { get; set; }

        [Required]
        public string CustomerUserEmail { get; set; }

        [Required]
        public string  DriverEmail { get; set; }

        [Required]
        public Rating Rating { get; set; }

        [StringLength(500)]
        public string Message { get; set; }




    }
}
