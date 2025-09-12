using System.ComponentModel.DataAnnotations;
using Uber.Uber.Application.Validations;

namespace Uber.Uber
{
    public class CreateOrUpdateCategoryDTO
    {
        [Required(ErrorMessage = "Category name is required.")]
        [MinLength(2, ErrorMessage = "Category name must be at least 2 characters.")]
        [MaxLength(50, ErrorMessage = "Category name must not exceed 50 characters.")]
        [UniqueCategoryName]
        public required string Name { get; set; }
    }
}
