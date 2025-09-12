using System.ComponentModel.DataAnnotations;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application.DTOs.PaymentDTOs
{
    public class UpdatePaymentDTO
    {
        [Required(ErrorMessage = "Please enter the payment method")]
        [EnumDataType(typeof(PaymentMethodEnum), ErrorMessage = "Invalid payment method")]
        [Display(Name = "Payment Method")]
        public PaymentMethodEnum PaymentMethod { get; set; }

        [Required(ErrorMessage = "Please enter the payment status")]
        [EnumDataType(typeof(PaymentStatus), ErrorMessage = "Invalid payment status")]
        [Display(Name = "Payment Status")]
        public PaymentStatus PaymentStatus { get; set; }

    }

 }
