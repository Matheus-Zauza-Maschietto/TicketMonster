using System;
using TicketMonster.Domain.Enums;

namespace TicketMonster.Domain;

public sealed class PaymentEntity
{
    public Guid Id { get; private set; }
    public int ValueInCents { get; private set; }
    public decimal Value { 
        get => ValueInCents / 100m; 
        private set => ValueInCents = (int)(value * 100);
    }
    public DateTime CreationDate { get; private set; }  
    public DateTime ExpirationDate { get; private set; }
    public PaymentStatus Status { get; private set; }
    public TicketEntity Ticket { get; private set; }
    public string ClientSecret { get; private set; }
    public Guid TicketId { get; private set; }

    private PaymentEntity(){}

    public PaymentEntity(decimal value, TicketEntity ticket)
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
        ExpirationDate = CreationDate.AddMinutes(15);
        Status = PaymentStatus.Pending;
        Value = value;
        Ticket = ticket;
        TicketId = ticket.Id;
    }   

    public void SetClientSecret(string clientSecret)
    {
        if(!string.IsNullOrEmpty(ClientSecret)) return;
        if(string.IsNullOrEmpty(clientSecret)) throw new ArgumentException("Client secret cannot be null or empty");
        
        ClientSecret = clientSecret;
    }

    public void MarkAsCompleted()
    {
        Status = PaymentStatus.Completed;
    }

    public void MarkAsFailed()
    {
        Status = PaymentStatus.Failed;
    }

}
