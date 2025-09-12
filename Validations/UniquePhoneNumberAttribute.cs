using System.ComponentModel.DataAnnotations;

namespace Uber.Uber.Application.Validations
{
    public class UniquePhoneNumberAttribute : ValidationAttribute
    {
        UberContext Context = new UberContext();
        protected override  ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value == null)
                return null;
            var dbContext = (UberContext)validationContext.GetService(typeof(UberContext));
             var currentPhone = value.ToString();
            var exists = dbContext.Users.Any(c => c.PhoneNumber == currentPhone);

            if (exists)
                return new ValidationResult("Phone  Number already exists.");

            return ValidationResult.Success;


        }
    }
}
