using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketMonster.DTOs;
using TicketMonster.Services;

namespace TicketMonster.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly TicketService _ticketService;

    public TicketsController(TicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketResponse>>> GetAll()
    {
        var tickets = await _ticketService.GetAllAsync();
        return Ok(tickets);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TicketResponse>> GetById(Guid id)
    {
        var ticket = await _ticketService.GetByIdAsync(id);
        if (ticket == null) return NotFound();

        return Ok(ticket);
    }

    [HttpGet("my-tickets")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<TicketResponse>>> GetMyTickets()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var tickets = await _ticketService.GetByUserIdAsync(userId);
        return Ok(tickets);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<TicketResponse>> Create([FromBody] CreateTicketRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var ticket = await _ticketService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<ActionResult> Delete(Guid id)
    {
        var deleted = await _ticketService.DeleteAsync(id);
        if (!deleted) return NotFound();

        return NoContent();
    }
}
