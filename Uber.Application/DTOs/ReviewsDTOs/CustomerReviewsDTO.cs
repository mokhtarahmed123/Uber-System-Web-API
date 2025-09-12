namespace Uber.Uber.Application.DTOs.ReviewsDTOs
{
    public class CustomerReviewsDTO
    {
        public string CustomerEmail { get; set; }
        public string CustomerName { get; set; }
        public List<ReviewDetailsDTO> Reviews { get; set; }
    }
}
