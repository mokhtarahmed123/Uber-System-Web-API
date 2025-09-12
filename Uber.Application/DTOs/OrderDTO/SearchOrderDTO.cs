namespace Uber.Uber.Application.DTOs.OrderDTO
{
    public class SearchOrderDTO
    {
        public string? CustomerEmail { get; set; }
        public string? MerchantEmail { get; set; }
        public string? Status { get; set; }
        public int? ItemId { get; set; }
    }
}
