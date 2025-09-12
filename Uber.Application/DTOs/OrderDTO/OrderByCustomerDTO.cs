using System.ComponentModel.DataAnnotations;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application.DTOs.OrderDTO
{
    public class OrderByCustomerDTO
    {
        [Display(Name = " Order ID  ")]

        public int OrderId { get; set; }

        [Display(Name = " Customer Email  ")]

        public string CustomerEmail { get; set; }

        [Display(Name = " Item Name")]

        public string ItemName { get; set; }

        [Display(Name = " Statue Of Order  ")]

        public OrderStatus Status { get; set; }
        [Display(Name = " Payment Method ")]
        public PaymentMethodEnum Payment { get; set; }

        public int TotalAmount { get; set; }
    }
}
