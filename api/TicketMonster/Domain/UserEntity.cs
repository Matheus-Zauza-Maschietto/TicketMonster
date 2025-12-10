using System;
using Microsoft.AspNetCore.Identity;

namespace TicketMonster.Domain;

public sealed class UserEntity : IdentityUser
{
    public ICollection<TicketEntity> Tickets { get; private set; } = [];
    public ICollection<ShowEntity> CreatedShows { get; private set; } = [];
}
