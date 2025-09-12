using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application.DTOs.ReviewsDTOs
{
    public class ReviewDetailsDTO
    {

        public int ID { get; set; }
        public string Message { get; set; }
        public Rating Rating { get; set; }
        public string CustomerName { get; set; }
        public string DriverName { get; set; }
        public int TripID { get; set; }


    }
}
