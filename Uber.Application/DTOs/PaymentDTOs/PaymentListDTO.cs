using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application
{
    public class PaymentListDTO
    {
        public int ID { get; set; }
        public double TotalPrice { get; set; }
        public PaymentMethodEnum Method { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
   
        public string UserEmail { get; set; }
        public string? MerchantName { get; set; }

    }
}
