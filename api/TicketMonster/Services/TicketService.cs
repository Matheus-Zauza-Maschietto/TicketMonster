using Microsoft.EntityFrameworkCore;
using TicketMonster.Domain;
using TicketMonster.DTOs;
using TicketMonster.Repositories;

namespace TicketMonster.Services;

public class TicketService
{
    private readonly ApplicationDbContext _context;

    public TicketService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TicketResponse>> GetAllAsync()
    {
        return await _context.Tickets
            .Select(t => new TicketResponse(
                t.Id,
                t.Code,
                t.UserOwnerId,
                t.ShowId,
                t.PaymentId
            ))
            .ToListAsync();
    }

    public async Task<TicketResponse?> GetByIdAsync(Guid id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null) return null;

        return new TicketResponse(
            ticket.Id,
            ticket.Code,
            ticket.UserOwnerId,
            ticket.ShowId,
            ticket.PaymentId
        );
    }

    public async Task<IEnumerable<TicketResponse>> GetByUserIdAsync(string userId)
    {
        return await _context.Tickets
            .Where(t => t.UserOwnerId == userId)
            .Select(t => new TicketResponse(
                t.Id,
                t.Code,
                t.UserOwnerId,
                t.ShowId,
                t.PaymentId
            ))
            .ToListAsync();
    }

    public async Task<TicketResponse> CreateAsync(CreateTicketRequest request, string userId)
    {
        UserEntity? user = await _context.Users.FindAsync(userId);
        if (user == null) throw new Exception("User not found");

        ShowEntity? show = await _context.Shows.FindAsync(request.ShowId);
        if (show == null) throw new Exception("Show not found");



        var ticket = new TicketEntity(user, show);

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return new TicketResponse(
            ticket.Id,
            ticket.Code,
            ticket.UserOwnerId,
            ticket.ShowId,
            ticket.PaymentId
        );
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null) return false;

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();

        return true;
    }
}
