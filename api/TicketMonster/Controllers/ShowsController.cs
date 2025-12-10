using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketMonster.DTOs;
using TicketMonster.Services;

namespace TicketMonster.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowsController : ControllerBase
{
    private readonly ShowService _showService;

    public ShowsController(ShowService showService)
    {
        _showService = showService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShowResponse>>> GetAll()
    {
        var shows = await _showService.GetAllAsync();
        return Ok(shows);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ShowResponse>> GetById(Guid id)
    {
        var show = await _showService.GetByIdAsync(id);
        if (show == null) return NotFound();

        return Ok(show);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ShowResponse>> Create([FromBody] CreateShowRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var show = await _showService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = show.Id }, show);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ShowResponse>> Update(Guid id, [FromBody] UpdateShowRequest request)
    {
        var show = await _showService.UpdateAsync(id, request);
        if (show == null) return NotFound();

        return Ok(show);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<ActionResult> Delete(Guid id)
    {
        var deleted = await _showService.DeleteAsync(id);
        if (!deleted) return NotFound();

        return NoContent();
    }
}
