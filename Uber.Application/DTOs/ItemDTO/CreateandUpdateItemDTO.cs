using System.ComponentModel.DataAnnotations;

namespace Uber.Uber
{
    public class CreateandUpdateItemDTO
    {
        [Required(ErrorMessage = "Item name is required.")]
        [MinLength(2, ErrorMessage = "Item name must be at least 2 characters.")]
        [MaxLength(50, ErrorMessage = "Item name must not exceed 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter the price")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        [Display(Name = "Item Price")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category  name is required.")]
        [Display(Name = "Category  Name")]
        public string CategoryName { get; set; }

        public int Quantity {  get; set; }

        [Required(ErrorMessage = "Merchan tEmail is required.")]
        [Display(Name = "Merchant Email ")]
        [DataType(DataType.EmailAddress)]
        public string MerchantEmail { get; set; }
    }
}
