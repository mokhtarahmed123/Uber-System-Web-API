using System.ComponentModel.DataAnnotations;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application.DTOs
{
    public class UpdateDriverProfile
    {
        public string VehicleType { get; set; }
        public string PlateNumber { get; set; }
        [DataType(DataType.Upload)]
        public string? LicenseImagePath { get; set; }
        public Rating Rating { get; set; }

    }
}
