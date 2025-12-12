using System;
using MassTransit;
using Stripe;
using TicketMonster.DTOs;
using TicketMonster.Services;

namespace TicketMonster.Consumers;

public class StripeWebhookConsumer : IConsumer<StripeWebhookDTO>
{
    private readonly PaymentService _paymentService;
    public StripeWebhookConsumer(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task Consume(ConsumeContext<StripeWebhookDTO> context)
    {
        await _paymentService.ProcessPaymentAsync(context.Message);
    }
}
