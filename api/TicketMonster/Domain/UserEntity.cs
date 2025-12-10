using System;
using Microsoft.AspNetCore.Identity;

namespace TicketMonster.Domain;

public class UserEntity : IdentityUser
{
    public ICollection<TicketEntity> Tickets { get; set; }
    public ICollection<ShowEntity> CreatedShows { get; set; }
}
