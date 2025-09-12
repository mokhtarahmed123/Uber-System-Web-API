using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uber.Uber.Domain.Entities
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter the name of the item")]
        [Display(Name = "Item Name")]
        [StringLength(100, ErrorMessage = "Item name can't exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter the price")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        [Display(Name = "Item Price")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [ForeignKey("category")]
        public int CategoryId { get; set; }
        public Category category { get; set; }

        [ForeignKey("merchant")]
        public int MerchantID { get; set; }
        public Merchant merchant { get; set; }

        public int Quantity { get; set; }

       public List<Order> Order { get; set; } = new List<Order>();

    }
}
