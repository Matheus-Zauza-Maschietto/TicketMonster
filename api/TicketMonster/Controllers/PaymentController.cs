using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketMonster.Services;

namespace TicketMonster.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly PaymentService _paymentService;

    public PaymentController(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            await _paymentService.ValidatePaymentWebhookAsync(json, Request.Headers?["Stripe-Signature"] ?? throw new InvalidOperationException("Signature header missing"));
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest();
        }
    }
}

