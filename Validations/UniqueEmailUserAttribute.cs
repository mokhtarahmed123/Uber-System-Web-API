using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Uber.Uber.Application.Validations
{
    public class UniqueEmailUserAttribute: ValidationAttribute
    {
        UberContext context = new UberContext();
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success; 

            var dbContext = (UberContext)validationContext.GetService(typeof(UberContext));

            if (dbContext == null)
                throw new InvalidOperationException("UberContext is not available in the ValidationContext.");

            string currentEmail = value.ToString();

            bool exists = dbContext.Users.Any(c => c.Email == currentEmail);

            if (exists)
                return new ValidationResult($"User with email '{currentEmail}' already exists.");

            return ValidationResult.Success;
        }
    }
}
