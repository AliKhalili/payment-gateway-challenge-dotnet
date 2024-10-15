using FluentAssertions;

using Microsoft.Extensions.Time.Testing;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Validations;

namespace PaymentGateway.Api.Tests.UnitTests
{
    public class PaymentRequestValidatorTests
    {
        private readonly FakeTimeProvider _timeProvider = new FakeTimeProvider(new DateTime(2024, 02, 01));
        private readonly PaymentRequestValidator _validator;
        public PaymentRequestValidatorTests()
        {
            _validator = new PaymentRequestValidator(_timeProvider);
        }

        [Fact]
        public void Should_Have_Error_When_CardNumber_Is_Empty()
        {
            var request = new PaymentRequest
            {
                CardNumber = "",
                ExpiryMonth = 12,
                ExpiryYear = 2025,
                Currency = PaymentCurrency.GBP,
                Amount = 100,
                Cvv = "123"
            };

            var result = _validator.Validate(request);

            result.Errors.Should().ContainSingle(e => e.PropertyName == "CardNumber" && e.ErrorMessage == "Card number is required.");
        }

        [Fact]
        public void Should_Have_Error_When_CardNumber_Is_Invalid_Length()
        {
            var request = new PaymentRequest
            {
                CardNumber = "123",
                ExpiryMonth = 12,
                ExpiryYear = 2025,
                Currency = PaymentCurrency.GBP,
                Amount = 100,
                Cvv = "123"
            };

            var result = _validator.Validate(request);

            result.Errors.Should().ContainSingle(e => e.PropertyName == "CardNumber" && e.ErrorMessage == "Card number must be between 14 and 19 characters long.");
        }

        [Fact]
        public void Should_Have_Error_When_CardNumber_Contains_Non_Numeric_Characters()
        {
            var request = new PaymentRequest
            {
                CardNumber = "1234abcd567890",
                ExpiryMonth = 12,
                ExpiryYear = 2025,
                Currency = PaymentCurrency.GBP,
                Amount = 100,
                Cvv = "123"
            };

            var result = _validator.Validate(request);

            result.Errors.Should().ContainSingle(e => e.PropertyName == "CardNumber" && e.ErrorMessage == "Card number must contain only numeric characters.");
        }

        [Fact]
        public void Should_Have_Error_When_ExpiryMonth_Is_Empty()
        {
            var request = new PaymentRequest
            {
                ExpiryMonth = 0,
                CardNumber = "1234567812345678",
                ExpiryYear = 2025,
                Currency = PaymentCurrency.GBP,
                Amount = 100,
                Cvv = "123"
            };

            var result = _validator.Validate(request);

            result.Errors.Should().ContainSingle(e => e.PropertyName == "ExpiryMonth" && e.ErrorMessage == "Expiry month is required.");
        }

        [Fact]
        public void Should_Have_Error_When_ExpiryMonth_Is_Invalid()
        {
            var request = new PaymentRequest
            {
                ExpiryMonth = 13,
                CardNumber = "1234567812345678",
                ExpiryYear = 2025,
                Currency = PaymentCurrency.GBP,
                Amount = 100,
                Cvv = "123"
            };

            var result = _validator.Validate(request);

            result.Errors.Should().ContainSingle(e => e.PropertyName == "ExpiryMonth" && e.ErrorMessage == "Expiry month must be between 1 and 12.");
        }

        [Fact]
        public void Should_Have_Error_When_ExpiryYear_Is_Empty()
        {
            var request = new PaymentRequest
            {
                ExpiryYear = 0,
                CardNumber = "1234567812345678",
                ExpiryMonth = 12,
                Currency = PaymentCurrency.GBP,
                Amount = 100,
                Cvv = "123"
            };

            var result = _validator.Validate(request);

            result.Errors.Should().ContainSingle(e => e.PropertyName == "ExpiryYear" && e.ErrorMessage == "Expiry year is required.");
        }

        [Fact]
        public void Should_Have_Error_When_ExpiryYear_Is_In_Past()
        {
            var request = new PaymentRequest
            {
                ExpiryMonth = 12,
                ExpiryYear = 2023,
                CardNumber = "1234567812345678",
                Currency = PaymentCurrency.GBP,
                Amount = 100,
                Cvv = "123"
            };

            var result = _validator.Validate(request);

            result.Errors.Should().ContainSingle(e => e.PropertyName == "ExpiryYear" && e.ErrorMessage == "Expiry year must be greater than or equal 2024.");
        }

        [Fact]
        public void Should_Have_Error_When_ExpiryDate_Is_In_The_Past()
        {
            var request = new PaymentRequest
            {
                ExpiryMonth = 01,
                ExpiryYear = 2024,
                CardNumber = "1234567812345678",
                Currency = PaymentCurrency.GBP,
                Amount = 100,
                Cvv = "123"
            };

            var result = _validator.Validate(request);

            result.Errors.Should().ContainSingle(e => e.PropertyName == "ExpiryYear" && e.ErrorMessage == "Expiry date must be in the future.");
        }

        [Fact]
        public void Should_Have_Error_When_Currency_Is_Invalid()
        {
            var request = new PaymentRequest
            {
                Currency = (PaymentCurrency)100,
                CardNumber = "1234567812345678",
                ExpiryMonth = 12,
                ExpiryYear = 2025,
                Amount = 100,
                Cvv = "123"
            };

            var result = _validator.Validate(request);

            result.Errors.Should().ContainSingle(e => e.PropertyName == "Currency" && e.ErrorMessage == "Currency must be a valid ISO currency code.");
        }

        [Fact]
        public void Should_Have_Error_When_Amount_Is_Zero()
        {
            var request = new PaymentRequest
            {
                Amount = 0,
                CardNumber = "1234567812345678",
                ExpiryMonth = 12,
                ExpiryYear = 2025,
                Currency = PaymentCurrency.GBP,
                Cvv = "123"
            };

            var result = _validator.Validate(request);

            result.Errors.Should().ContainSingle(e => e.PropertyName == "Amount" && e.ErrorMessage == "Amount must be an integer and greater than zero.");
        }

        [Fact]
        public void Should_Have_Error_When_Cvv_Is_Empty()
        {
            var request = new PaymentRequest
            {
                Cvv = "",
                CardNumber = "1234567812345678",
                ExpiryMonth = 12,
                ExpiryYear = 2025,
                Currency = PaymentCurrency.GBP,
                Amount = 100
            };

            var result = _validator.Validate(request);

            result.Errors.Should().ContainSingle(e => e.PropertyName == "Cvv" && e.ErrorMessage == "CVV is required.");
        }

        [Fact]
        public void Should_Have_Error_When_Cvv_Is_Invalid_Length()
        {
            var request = new PaymentRequest
            {
                Cvv = "12",
                CardNumber = "1234567812345678",
                ExpiryMonth = 12,
                ExpiryYear = 2025,
                Currency = PaymentCurrency.GBP,
                Amount = 100
            };

            var result = _validator.Validate(request);

            result.Errors.Should().ContainSingle(e => e.PropertyName == "Cvv" && e.ErrorMessage == "CVV must be 3 or 4 characters long.");
        }

        [Fact]
        public void Should_Have_Error_When_Cvv_Contains_Non_Numeric_Characters()
        {
            var request = new PaymentRequest
            {
                Cvv = "12a",
                CardNumber = "1234567812345678",
                ExpiryMonth = 12,
                ExpiryYear = 2025,
                Currency = PaymentCurrency.GBP,
                Amount = 100
            };

            var result = _validator.Validate(request);

            result.Errors.Should().ContainSingle(e => e.PropertyName == "Cvv" && e.ErrorMessage == "CVV must contain only numeric characters.");
        }

        [Fact]
        public void Should_Pass_When_All_Fields_Are_Valid()
        {
            var request = new PaymentRequest
            {
                CardNumber = "1234567812345678",
                ExpiryMonth = 12,
                ExpiryYear = 2025,
                Currency = PaymentCurrency.GBP,
                Amount = 100,
                Cvv = "123"
            };

            var result = _validator.Validate(request);

            result.IsValid.Should().BeTrue();
        }
    }
}