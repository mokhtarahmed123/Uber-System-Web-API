using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Domain.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [DataType(DataType.Date)]
        public DateOnly OrderDate { get; set; }

        [Required(ErrorMessage = " Enter The Payment Method ")]
        [Display(Name = "Payment Method ")]
        public PaymentMethodEnum PaymentMethod { get; set; }

        [Required(ErrorMessage = " Enter The Sataus ")]
        [Display(Name = " Order  Status  ")]
        public OrderStatus Status { get; set; }

        public int totalAmount { get; set; }

        [Required]
        [ForeignKey("user")]
        public int CustometID { get; set; }
        public Customer user { get; set; }

        [Required]
        [ForeignKey("merchant")]
        public int MerchantID { get; set; }
        public Merchant merchant { get; set; }

        [Required]
        [ForeignKey("item")]
        public int ItemID { get; set; }
        public Item item { get; set; }

        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();

    }
}
