namespace Uber.Uber.Application.DTOs.ComplaintsDTOs
{
    public class SearchComplaintDTO
    {
        public string? FromUserEmail { get; set; }
        public string? AgainstEmail { get; set; }
        public int? TripID { get; set; }
        public bool? IsResolved { get; set; }
    }
}
