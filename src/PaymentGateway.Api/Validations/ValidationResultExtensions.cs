using FluentValidation.Results;

namespace PaymentGateway.Api.Validations
{
    public static class ValidationResultExtensions
    {
        public static ValidationResult CreateValidationResult(this Exception exception)
        {
            // Create a ValidationFailure based on the Exception message
            return new ValidationResult(new List<ValidationFailure>
            {
                new("Error", exception.Message)
            });
        }

        public static ValidationResult CreateValidationResult(this string errorMessage)
        {
            // Create a ValidationFailure based on the provided error message
            return new ValidationResult(new List<ValidationFailure>
            {
                new("Error", errorMessage)
            });
        }
    }
}