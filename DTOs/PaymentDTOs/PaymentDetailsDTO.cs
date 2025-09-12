using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application.DTOs.PaymentDTOs
{
    public class PaymentDetailsDTO
    {
        public double TotalPrice { get; set; }
        public PaymentMethodEnum Method { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public int? CustomerId { get; set; }
        public string? UserName { get; set; }
        public int MerchantId { get; set; }
        public string MerchantName { get; set; }
        public int TripID {  get; set; }  
    }
}
