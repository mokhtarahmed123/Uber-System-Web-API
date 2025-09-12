using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application.DTOs.PaymentDTOs
{
    public class SearchPaymentDTO
    {
        public string? CustomerEmail { get; set; }
        public PaymentMethodEnum? Method { get; set; }        // Cash, Visa
        public PaymentStatus? PaymentStatus { get; set; } // Completed, Pending, Failed
    }
}
