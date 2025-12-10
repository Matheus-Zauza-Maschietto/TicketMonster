using System;
using Microsoft.EntityFrameworkCore;
using Stripe;
using TicketMonster.Domain;
using TicketMonster.Repositories;

namespace TicketMonster.Services;

public class PaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    public PaymentService(IConfiguration configuration, ApplicationDbContext context)
    {
        _context = context;
        _configuration = configuration;
        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];    
    }

    public async Task<PaymentIntent> CreatePaymentIntentAsync(PaymentEntity payment)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = payment.ValueInCents,
            Currency = "brl",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },        
            Metadata = new Dictionary<string, string>
            {
                { "paymentId", payment.Id.ToString() },
                { "ticketId", payment.Ticket.Id.ToString() }
            }
        };
        var service = new PaymentIntentService();
        PaymentIntent paymentIntent = await service.CreateAsync(options);
        return paymentIntent;
    }

    public async Task CancelExpiredPaymentsAsync()
    {
        var expiredPayments = await _context.Payments
            .Where(p => p.Status == Domain.Enums.PaymentStatus.Pending && p.ExpirationDate <= DateTime.UtcNow)
            .ToListAsync();

        foreach (var payment in expiredPayments)
        {
            payment.MarkAsFailed();
        }

        await _context.SaveChangesAsync();
    }

    public async Task ValidatePaymentWebhookAsync(string json, string sigHeader)
    {
        var endpointSecret = _configuration["Stripe:WebhookSecret"];

        Event stripeEvent = EventUtility.ConstructEvent(
            json,
            sigHeader,
            endpointSecret
        );

        PaymentIntent paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
        if (!(paymentIntent.Metadata.TryGetValue("paymentId", out var paymentIdString) && Guid.TryParse(paymentIdString, out var paymentId)))
        {
            throw new Exception("Invalid payment ID");
        }

        var payment = await _context.Payments.FindAsync(paymentId);
        if (payment == null)
        {
            throw new Exception("Payment not found");
        }

        if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
        {
            payment.MarkAsCompleted();
        }
        else if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
        {
            payment.MarkAsFailed();
        }
        else
        {
            throw new Exception("Invalid event type");
        }

        await _context.SaveChangesAsync();
    }
}
