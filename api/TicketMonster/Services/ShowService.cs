using Microsoft.EntityFrameworkCore;
using TicketMonster.Domain;
using TicketMonster.DTOs;
using TicketMonster.Repositories;

namespace TicketMonster.Services;

public class ShowService
{
    private readonly ApplicationDbContext _context;

    public ShowService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ShowResponse>> GetAllAsync()
    {
        return await _context.Shows
            .Select(s => new ShowResponse(
                s.Id,
                s.Title,
                s.TicketPrice,
                s.Singer,
                s.PresentationDate,
                s.MaxTicketQuantity,
                s.UserCreatorId
            ))
            .ToListAsync();
    }

    public async Task<ShowResponse?> GetByIdAsync(Guid id)
    {
        var show = await _context.Shows.FindAsync(id);
        if (show == null) return null;

        return new ShowResponse(
            show.Id,
            show.Title,
            show.TicketPrice,
            show.Singer,
            show.PresentationDate,
            show.MaxTicketQuantity,
            show.UserCreatorId
        );
    }

    public async Task<ShowResponse> CreateAsync(CreateShowRequest request, string userId)
    {
        var show = new ShowEntity
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            TicketPrice = request.TicketPrice,
            Singer = request.Singer,
            PresentationDate = request.PresentationDate,
            MaxTicketQuantity = request.MaxTicketQuantity,
            UserCreatorId = userId
        };

        _context.Shows.Add(show);
        await _context.SaveChangesAsync();

        return new ShowResponse(
            show.Id,
            show.Title,
            show.TicketPrice,
            show.Singer,
            show.PresentationDate,
            show.MaxTicketQuantity,
            show.UserCreatorId
        );
    }

    public async Task<ShowResponse?> UpdateAsync(Guid id, UpdateShowRequest request)
    {
        var show = await _context.Shows.FindAsync(id);
        if (show == null) return null;

        show.Title = request.Title;
        show.TicketPrice = request.TicketPrice;
        show.Singer = request.Singer;
        show.PresentationDate = request.PresentationDate;
        show.MaxTicketQuantity = request.MaxTicketQuantity;

        await _context.SaveChangesAsync();

        return new ShowResponse(
            show.Id,
            show.Title,
            show.TicketPrice,
            show.Singer,
            show.PresentationDate,
            show.MaxTicketQuantity,
            show.UserCreatorId
        );
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var show = await _context.Shows.FindAsync(id);
        if (show == null) return false;

        _context.Shows.Remove(show);
        await _context.SaveChangesAsync();

        return true;
    }
}

