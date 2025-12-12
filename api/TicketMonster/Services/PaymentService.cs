using System;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Stripe;
using TicketMonster.Domain;
using TicketMonster.DTOs;
using TicketMonster.Repositories;

namespace TicketMonster.Services;

public class PaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IPublishEndpoint _publishEndpoint;
    public PaymentService(
        IConfiguration configuration, 
        ApplicationDbContext context, 
        IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _configuration = configuration;
        _publishEndpoint = publishEndpoint;
        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];    
    }

    public async Task<IEnumerable<PaymentResponse>> GetUserPaymentsAsync(string userId)
    {
        return await _context.Payments
            .Where(p => p.Ticket.UserOwnerId == userId)
            .Select(p => new PaymentResponse(
                p.Id,
                p.Value,
                p.CreationDate,
                p.ExpirationDate,
                p.Status,
                p.ClientSecret,
                p.TicketId
            ))
            .ToListAsync();
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

        payment.SetClientSecret(paymentIntent.ClientSecret ?? string.Empty);

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

        Stripe.Event stripeEvent = EventUtility.ConstructEvent(
            json,
            sigHeader,
            endpointSecret
        );

        PaymentIntent paymentIntent = (PaymentIntent)stripeEvent.Data.Object;

        await _publishEndpoint.Publish((paymentIntent, stripeEvent));
    }

    public async Task ProcessPaymentAsync(StripeWebhookDTO webhook)
    {

        if (!(webhook.PaymentIntent.Metadata.TryGetValue("paymentId", out var paymentIdString) && Guid.TryParse(paymentIdString, out var paymentId)))
        {
            throw new Exception("Invalid payment ID");
        }

        var payment = await _context.Payments.FindAsync(paymentId);
        if (payment == null)
        {
            throw new Exception("Payment not found");
        }

        if (webhook.StripeEvent.Type == EventTypes.PaymentIntentSucceeded)
        {
            payment.MarkAsCompleted();
        }
        else if (webhook.StripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
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
