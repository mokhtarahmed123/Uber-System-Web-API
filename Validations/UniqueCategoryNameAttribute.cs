using System.ComponentModel.DataAnnotations;

namespace Uber.Uber.Application.Validations
{

    public class UniqueCategoryNameAttribute : ValidationAttribute
    {
        UberContext Context = new UberContext();
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return null;
            var dbContext = (UberContext)validationContext.GetService(typeof(UberContext));
            var currentName = value.ToString().ToLower();
            var exists = dbContext.Categories.Any(c => c.Name.ToLower() == currentName);

            if (exists)
                return new ValidationResult("Category name already exists.");

            return ValidationResult.Success;
        }
    }
}
