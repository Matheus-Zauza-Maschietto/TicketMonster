using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMonster.Domain;

public sealed class ShowEntity
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public decimal TicketPrice { get; private set; }
    public string Singer { get; private set; }
    public DateTime PresentationDate { get; private set; }
    public long MaxTicketQuantity { get; private set; }
    public UserEntity UserCreator { get; private set; }
    public string UserCreatorId { get; private set; }
    public ICollection<TicketEntity> Tickets { get; set; } = new List<TicketEntity>();

    private ShowEntity() { }

    public ShowEntity(string title, string singer, DateTime presentationDate, long maxTicketQuantity, UserEntity userCreator)
    {
        Id = Guid.NewGuid();
        Title = title;
        Singer = singer;
        PresentationDate = presentationDate;
        MaxTicketQuantity = maxTicketQuantity;
        UserCreator = userCreator;
        UserCreatorId = userCreator.Id;
    }

    public void UpdateDetails(string title, string singer, DateTime presentationDate, long maxTicketQuantity)
    {
        Title = title;
        Singer = singer;
        PresentationDate = presentationDate;
        MaxTicketQuantity = maxTicketQuantity;
    }
}
