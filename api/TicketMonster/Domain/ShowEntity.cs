using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMonster.Domain;

public class ShowEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public decimal TicketPrice { get; set; }
    public string Singer { get; set; }
    public DateTime PresentationDate { get; set; }
    public long MaxTicketQuantity { get; set; }
    public UserEntity UserCreator { get; set; }
    public string UserCreatorId { get; set; }
    public ICollection<TicketEntity> Tickets { get; set; }

}
