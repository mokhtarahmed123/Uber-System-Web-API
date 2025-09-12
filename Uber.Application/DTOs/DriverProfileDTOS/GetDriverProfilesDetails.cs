using System.ComponentModel.DataAnnotations;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application.DTOs
{
    public class GetDriverProfilesDetails
    {
        public string Name { get; set; }
        public string Email {  get; set; }
        public string VehicleType { get; set; }
        public string PlateNumber { get; set; }
        [DataType(DataType.Upload)]
        public string? LicenseImagePath { get; set; }
        public int NumberOfTrips { get; set; }
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public string Status { get; set; }



    }
}
