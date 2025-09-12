namespace Uber.Uber.Application.DTOs.ReviewsDTOs
{
    public class DriverReviewsDTO
    {
        public int DriverID { get; set; }
        public string DriverName { get; set; }
        public double AverageRating { get; set; }
        public List<ReviewDetailsDTO> Reviews { get; set; }
    }
}
