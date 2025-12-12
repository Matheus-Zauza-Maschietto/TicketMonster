using System;
using Stripe;

namespace TicketMonster.DTOs;

public class StripeWebhookDTO
{
    public PaymentIntent PaymentIntent { get; set; }
    public Event StripeEvent { get; set; }
}
