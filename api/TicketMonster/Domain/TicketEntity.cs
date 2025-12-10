using System;

namespace TicketMonster.Domain;

public sealed class TicketEntity
{
    public Guid Id { get; private set; }
    public Guid Code { get; private set; }
    public UserEntity UserOwner { get; private set; }
    public string UserOwnerId { get; private set; }
    public ShowEntity Show { get; private set; }
    public Guid ShowId { get; private set; }
    public PaymentEntity Payment { get; private set; }
    public Guid PaymentId { get; private set; }

    private TicketEntity() { }

    public TicketEntity(UserEntity userOwner, ShowEntity show)
    {
        Id = Guid.NewGuid();
        Code = Guid.NewGuid();
        UserOwner = userOwner;
        UserOwnerId = userOwner.Id;
        Show = show;
        ShowId = show.Id;
        Payment = new PaymentEntity(show.TicketPrice, this);
        PaymentId = Payment.Id;
    }
}
