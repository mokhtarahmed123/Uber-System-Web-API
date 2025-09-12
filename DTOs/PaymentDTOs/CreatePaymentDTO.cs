using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application.DTOs.PaymentDTOs
{
    public class CreatePaymentDTO
    {
        [Required(ErrorMessage = " Please Enter Method Of Payment ")]
        [Display(Name = " Payment Method ")]
        public PaymentMethodEnum PaymentMethod { get; set; }
        [Required(ErrorMessage = " Please Enter Statue Of Payment ")]
        [Display(Name = " Payment Statue ")]
        public PaymentStatus PaymentStatus { get; set; }
        //[Required(ErrorMessage = " Please Enter Total Price  ")]
        [JsonIgnore]
        public int TotalPrice { get; set; }
        public int TripID { get; set; }

        public string CustomerEmail {  get; set; }
        public string? MerchantEmail {  get; set; }

    }
}
