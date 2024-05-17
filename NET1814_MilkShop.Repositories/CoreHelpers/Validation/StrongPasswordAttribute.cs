using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.CoreHelpers.Validation
{
    internal class StrongPasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var password = value as string;

            if (password == null)
            {
                return new ValidationResult("Password is required.");
            }

            if (password.Length < 7 || password.Length > 12)
            {
                return new ValidationResult("Password must be 7-12 characters long.");
            }

            if (!password.Any(char.IsUpper))
            {
                return new ValidationResult("Password must contain at least one uppercase letter.");
            }

            if (!password.Any(char.IsLower))
            {
                return new ValidationResult("Password must contain at least one lowercase letter.");
            }

            if (!password.Any(char.IsDigit))
            {
                return new ValidationResult("Password must contain at least one digit.");
            }

            if (password.IndexOfAny("!@#$%^&*()".ToCharArray()) == -1)
            {
                return new ValidationResult("Password must contain at least one special character.");
            }

            return ValidationResult.Success;
        }
    }
}
