using System;
using TicketMonster.Domain.Enums;

namespace TicketMonster.Domain;

public class PaymentEntity
{
    public Guid Id { get; set; }
    public decimal Value { get; set; }
    public DateTime CreationDate { get; set; }  
    public PaymentStatus Status { get; set; }
    public TicketEntity Ticket { get; set; }
    public Guid TicketId { get; set; }
}
