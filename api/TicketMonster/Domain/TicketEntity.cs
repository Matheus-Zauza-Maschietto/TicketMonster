using System;

namespace TicketMonster.Domain;

public class TicketEntity
{
    public Guid Id { get; set; }
    public Guid Code { get; set; }
    public UserEntity UserOwner { get; set; }
    public string UserOwnerId { get; set; }
    public ShowEntity Show { get; set; }
    public Guid ShowId { get; set; }
    public PaymentEntity Payment { get; set; }
    public Guid PaymentId { get; set; }
}
