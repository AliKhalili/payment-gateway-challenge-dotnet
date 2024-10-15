using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Applications.Payment;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Controllers;

[Route("api/payments")]
[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
public class PaymentsController : Controller
{
    private readonly PaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(PaymentService paymentService, ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// Enables merchants to fetch details of a past payment using its payment id
    /// </summary>
    /// <param name="id">The payment Id</param>
    /// <returns>Return the existing payment record.</returns>
    /// <response code="200">Returns a response for existing payment.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/payments/{:id}
    /// </remarks>
    /// <response code="404">Not Found.</response>
    [HttpGet("{id}")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(typeof(GetPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentAsync(string id)
    {
        var response = _paymentService.GetPayment(id);

        if (response is null)
        {
            return NotFound();
        }

        return new OkObjectResult(response);
    }


    /// <summary>
    /// Allows merchants to submit requests to the payment gateway for processing card payments.
    /// </summary>
    /// <param name="paymentId">An optional idempotency key in header for safely retrying payment requests.</param>
    /// <param name="request">The given payment request.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the operation.</param>
    /// <returns>Returns a response for successfully submitted payments.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/payments
    ///     {
    ///         "card_number": "2222405343248877",
    ///         "expiry_month": 4,
    ///         "expiry_year": 2025,
    ///         "currency": "GBP",
    ///         "amount": 100,
    ///         "cvv": "123"
    ///     }
    /// </remarks>
    /// <response code="201">Returns a response for successfully submitted payments</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="429">Duplicate request detected.</response>
    [HttpPost]
    [ProducesDefaultResponseType]
    [ProducesResponseType(typeof(PostPaymentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> ProcessNewPaymentAsync(
        [FromHeader(Name = "Cko-Idempotency-Key")] string? paymentId,
        PaymentRequest request,
        CancellationToken cancellationToken)
    {
        var checkDuplicatePayment = _paymentService.IsPaymentDuplicate(paymentId);

        return await checkDuplicatePayment.Match(
            isDuplicated => Task.FromResult((IActionResult)StatusCode(StatusCodes.Status429TooManyRequests, "Duplicate request detected")),
            async paymentId =>
                {
                    var paymentResult = await _paymentService.ProcessPaymentAsync(paymentId, request, cancellationToken);
                    var response = paymentResult.Match<IActionResult>(
                        rejected =>
                        {
                            rejected.ValidationResult.AddToModelState(ModelState);
                            return BadRequest(ModelState);
                        },
                        done =>
                        {
                            var model = done.PostPaymentResponse;
                            return Ok(model);
                        });
                    return response;
                }
            );
    }
}