using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Tests.FunctionalTests
{
    public class PaymentsApiTests
    {
        private readonly WebApplicationFactory<Program> _factory = new();


        [Fact]
        public async Task Create_a_Valid_Payment_and_Retrieve_It()
        {
            // Arrange
            PaymentRequest paymentRequest = new()
            {
                CardNumber = "2222405343248877",
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Currency = PaymentCurrency.GBP,
                Amount = 100,
                Cvv = "123"
            };
            using var httpClient = _factory.CreateClient();


            // Act 
            var paymentResponse = await httpClient.PostAsJsonAsync($"/api/Payments", paymentRequest);
            var paymentResult = await paymentResponse.Content.ReadFromJsonAsync<PostPaymentResponse>();

            var getResponse = await httpClient.GetAsync($"/api/Payments/{paymentResult!.Id}");
            var getResult = await getResponse.Content.ReadFromJsonAsync<GetPaymentResponse>();


            // Assert

            paymentResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            paymentResult.Should().BeEquivalentTo(new PostPaymentResponse
            {
                CardNumberLastFour = 8877,
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Currency = PaymentCurrency.GBP.ToString(),
                Amount = 100,
                Status = PaymentStatus.Authorized,
                Id = paymentResult!.Id
            });
            getResult.Should().BeEquivalentTo(new GetPaymentResponse
            {
                CardNumberLastFour = 8877,
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Currency = PaymentCurrency.GBP.ToString(),
                Amount = 100,
                Status = PaymentStatus.Authorized,
                Id = paymentResult!.Id
            });
        }

        [Fact]
        public async Task Create_a_Valid_Payment_with_payment_id_and_Retrieve_It()
        {
            // Arrange
            string paymentId = "random_payment_id";
            PaymentRequest paymentRequest = new()
            {
                CardNumber = "2222405343248877",
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Currency = PaymentCurrency.GBP,
                Amount = 100,
                Cvv = "123"
            };
            using var httpClient = _factory.CreateClient();


            // Act 
            httpClient.DefaultRequestHeaders.Add("Cko-Idempotency-Key", paymentId);
            var paymentResponse = await httpClient.PostAsJsonAsync($"/api/Payments", paymentRequest);
            var paymentResult = await paymentResponse.Content.ReadFromJsonAsync<PostPaymentResponse>();

            var getResponse = await httpClient.GetAsync($"/api/Payments/{paymentId}");
            var getResult = await getResponse.Content.ReadFromJsonAsync<GetPaymentResponse>();


            // Assert

            paymentResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            paymentResult.Should().BeEquivalentTo(new PostPaymentResponse
            {
                CardNumberLastFour = 8877,
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Currency = PaymentCurrency.GBP.ToString(),
                Amount = 100,
                Status = PaymentStatus.Authorized,
                Id = paymentId
            });
            getResult.Should().BeEquivalentTo(new GetPaymentResponse
            {
                CardNumberLastFour = 8877,
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Currency = PaymentCurrency.GBP.ToString(),
                Amount = 100,
                Status = PaymentStatus.Authorized,
                Id = paymentId
            });
        }

        [Fact]
        public async Task Create_a_Duplicate_Payment_with_payment_id_and_Retrieve_It()
        {
            // Arrange
            string paymentId = "random_payment_id";
            PaymentRequest paymentRequest = new()
            {
                CardNumber = "2222405343248877",
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Currency = PaymentCurrency.GBP,
                Amount = 100,
                Cvv = "123"
            };
            using var httpClient = _factory.CreateClient();


            // Act 
            httpClient.DefaultRequestHeaders.Add("Cko-Idempotency-Key", paymentId);
            var paymentResponse1 = await httpClient.PostAsJsonAsync($"/api/Payments", paymentRequest);
            var paymentResult1 = await paymentResponse1.Content.ReadFromJsonAsync<PostPaymentResponse>();

            var paymentResponse2 = await httpClient.PostAsJsonAsync($"/api/Payments", paymentRequest);


            // Assert

            paymentResponse1.StatusCode.Should().Be(HttpStatusCode.OK);
            paymentResponse2.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
            paymentResult1.Should().BeEquivalentTo(new PostPaymentResponse
            {
                CardNumberLastFour = 8877,
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Currency = PaymentCurrency.GBP.ToString(),
                Amount = 100,
                Status = PaymentStatus.Authorized,
                Id = paymentId
            });
        }

        [Fact]
        public async Task Create_a_declined_Payment_and_Retrieve_It()
        {
            // Arrange
            PaymentRequest paymentRequest = new()
            {
                CardNumber = "2222405343248112",
                ExpiryMonth = 1,
                ExpiryYear = 2026,
                Currency = PaymentCurrency.USD,
                Amount = 60000,
                Cvv = "456"
            };
            using var httpClient = _factory.CreateClient();


            // Act 
            var paymentResponse = await httpClient.PostAsJsonAsync($"/api/Payments", paymentRequest);
            var paymentResult = await paymentResponse.Content.ReadFromJsonAsync<PostPaymentResponse>();

            var getResponse = await httpClient.GetAsync($"/api/Payments/{paymentResult!.Id}");
            var getResult = await getResponse.Content.ReadFromJsonAsync<GetPaymentResponse>();


            // Assert

            paymentResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            paymentResult.Should().BeEquivalentTo(new PostPaymentResponse
            {
                CardNumberLastFour = 8112,
                ExpiryMonth = 1,
                ExpiryYear = 2026,
                Currency = PaymentCurrency.USD.ToString(),
                Amount = 60000,
                Status = PaymentStatus.Declined,
                Id = paymentResult!.Id
            });
            getResult.Should().BeEquivalentTo(new GetPaymentResponse
            {
                CardNumberLastFour = 8112,
                ExpiryMonth = 1,
                ExpiryYear = 2026,
                Currency = PaymentCurrency.USD.ToString(),
                Amount = 60000,
                Status = PaymentStatus.Declined,
                Id = paymentResult!.Id
            });
        }

        [Fact]
        public async Task Create_a_rejected_Payment()
        {
            // Arrange
            PaymentRequest paymentRequest = new()
            {
                CardNumber = "2222405343248112",
                ExpiryMonth = 1,
                ExpiryYear = 2023, // invalid year
                Currency = PaymentCurrency.USD,
                Amount = 60000,
                Cvv = "456"
            };
            using var httpClient = _factory.CreateClient();


            // Act 
            var paymentResponse = await httpClient.PostAsJsonAsync($"/api/Payments", paymentRequest);
            var errorResult = await paymentResponse.Content.ReadAsStringAsync();


            // Assert

            paymentResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            errorResult.Should().Be("{\"ExpiryYear\":[\"Expiry year must be greater than or equal 2024.\",\"Expiry date must be in the future.\"]}");
        }

        [Fact]
        public async Task Retrieve_a_not_found_Payment()
        {
            // Arrange
            string paymentId = "not_found_payment";
            using var httpClient = _factory.CreateClient();


            // Act 
            var getResponse = await httpClient.GetAsync($"/api/Payments/{paymentId}");


            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}