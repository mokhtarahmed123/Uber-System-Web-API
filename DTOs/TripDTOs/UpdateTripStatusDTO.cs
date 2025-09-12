using System.ComponentModel.DataAnnotations;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application.DTOs.TripDTOs
{
    public class UpdateTripStatusDTO
    {
        [Required(ErrorMessage = "Trip status is required.")]
        public StatausTrip StatausTrip { get; set; }
    }
}
