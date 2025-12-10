using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketMonster.DTOs;
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

    [HttpGet()]
    [Authorize]
    public async Task<ActionResult<IEnumerable<PaymentResponse>>> GetMyPayments()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var payments = await _paymentService.GetUserPaymentsAsync(userId);
        return Ok(payments);
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var signature = Request.Headers["Stripe-Signature"].ToString();
            if (string.IsNullOrEmpty(signature))
                return BadRequest("Signature header missing");

            await _paymentService.ValidatePaymentWebhookAsync(json, signature);
            return Ok();
        }
        catch
        {
            return BadRequest();
        }
    }
}

