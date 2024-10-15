using FluentValidation;

using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.Validations
{
    public class PaymentRequestValidator : AbstractValidator<PaymentRequest>
    {
        private readonly TimeProvider _timeProvider;
        public PaymentRequestValidator(TimeProvider timeProvider)
        {
            _timeProvider = timeProvider;

            RuleFor(request => request.CardNumber)
                .NotEmpty().WithMessage("Card number is required.")
                .Length(14, 19).WithMessage("Card number must be between 14 and 19 characters long.")
                .Matches("^[0-9]*$").WithMessage("Card number must contain only numeric characters.");

            RuleFor(request => request.ExpiryMonth)
                .NotEmpty().WithMessage("Expiry month is required.")
                .InclusiveBetween(1, 12).WithMessage("Expiry month must be between 1 and 12.");

            RuleFor(request => request.ExpiryYear)
                .NotEmpty().WithMessage("Expiry year is required.")
                .GreaterThanOrEqualTo(2024).WithMessage("Expiry year must be greater than or equal 2024.")
                .Must((request, expiryYear) => IsExpiryDateValid(request.ExpiryMonth, expiryYear)).WithMessage("Expiry date must be in the future.");

            RuleFor(request => request.Currency)
                .IsInEnum().WithMessage("Currency must be a valid ISO currency code.");

            RuleFor(request => request.Amount)
                .NotEmpty().WithMessage("Amount is required.")
                .GreaterThan(0).WithMessage("Amount must be an integer and greater than zero.");

            RuleFor(request => request.Cvv)
                .NotEmpty().WithMessage("CVV is required.")
                .Length(3, 4).WithMessage("CVV must be 3 or 4 characters long.")
                .Matches("^[0-9]*$").WithMessage("CVV must contain only numeric characters.");
        }

        private bool IsExpiryDateValid(int month, int year)
        {
            try
            {
                var currentDate = _timeProvider.GetUtcNow();
                var expiryDate = new DateTime(year, month, 1);

                // Adjust to the end of the month for correct comparison
                expiryDate = expiryDate.AddMonths(1).AddDays(-1);

                return expiryDate > currentDate;
            }
            catch
            {
                return false;
            }
        }
    }
}