using Microsoft.EntityFrameworkCore;
using TicketMonster.Domain;
using TicketMonster.Domain.Enums;
using TicketMonster.DTOs;
using TicketMonster.Repositories;

namespace TicketMonster.Services;

public class TicketService
{
    private readonly ApplicationDbContext _context;
    private readonly PaymentService _paymentService;

    public TicketService(ApplicationDbContext context, PaymentService paymentService)
    {
        _context = context;
        _paymentService = paymentService;
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

        int ticketQuantity = await _context.Tickets.CountAsync(t => t.ShowId == show.Id && t.Payment.Status == PaymentStatus.Completed || (t.Payment.Status == PaymentStatus.Pending && t.Payment.ExpirationDate > DateTime.UtcNow));
        if (ticketQuantity >= show.MaxTicketQuantity) throw new Exception("Show is full");

        var ticket = new TicketEntity(user, show);

        _context.Tickets.Add(ticket);

        await _paymentService.CreatePaymentIntentAsync(ticket.Payment);

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
        var ticket = await _context.Tickets.Include(p => p.Payment).FirstOrDefaultAsync(t => t.Id == id);
        if (ticket == null) return false;

        ticket.Payment.MarkAsFailed();

        _context.Tickets.Remove(ticket);

        await _context.SaveChangesAsync();

        return true;
    }
}
