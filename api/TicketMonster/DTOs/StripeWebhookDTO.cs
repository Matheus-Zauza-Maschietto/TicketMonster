using System;
using Stripe;

namespace TicketMonster.DTOs;

public class StripeWebhookDTO
{
    public string PaymentId { get; set; }
    public string EventType { get; set; }
}
