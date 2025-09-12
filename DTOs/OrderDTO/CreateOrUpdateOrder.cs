using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Uber.Uber.Domain.Entities;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application
{
    public class CreateOrUpdateOrder
    {


        [Required(ErrorMessage = "Order Date is required.")]
        [Display(Name = "Order Date")]
        [DataType(DataType.Date)]
        public DateOnly OrderDate { get; set; }

        [Display(Name = "Total Amount")]
        public int TotalAmount { get; set; }

        [Required(ErrorMessage = "Please enter the name of the item.")]
        [Display(Name = "Item Name")]
        [StringLength(100, ErrorMessage = "Item name cannot exceed 100 characters.")]
        public string ItemName { get; set; }

        [Required(ErrorMessage = "Please select a payment method.")]
        [Display(Name = "Payment Method")]
        public PaymentMethodEnum PaymentMethod { get; set; }

        [Required(ErrorMessage = "Please select the order status.")]
        [Display(Name = "Order Status")]
        public OrderStatus Status { get; set; }

        [Required(ErrorMessage = "Please enter the customer's email.")]
        [Display(Name = "Customer Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please enter a valid customer email.")]
        public string CustomerEmail { get; set; }

        [Display(Name = "Merchant Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please enter a valid merchant email.")]
        [JsonIgnore]
        public string? MerchantEmail { get; set; }




    }
}
