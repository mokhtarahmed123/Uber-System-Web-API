using System.ComponentModel.DataAnnotations;

namespace Uber.Uber.Application.DTOs.CategoryDTO
{
    public class GetCategoryWithItems
    {
        [Required]
        [MinLength(length: 2, ErrorMessage = " Name Must be Greater Than 2 ")]
        public string CategoryName { get; set; }
        public int ItemsNumber { get; set; }
        public string ItemsName { get; set; }
        public int Quanitity { get; set; }


    }
}
