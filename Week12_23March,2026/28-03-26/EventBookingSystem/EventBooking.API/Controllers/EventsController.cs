using AutoMapper;
using EventBooking.API.Data;
using EventBooking.API.DTOs;
using EventBooking.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBooking.API.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public EventsController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // GET api/events
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetAll()
    {
        var events = await _db.Events.ToListAsync();
        return Ok(_mapper.Map<IEnumerable<EventDto>>(events));
    }

    // GET api/events/{id}
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<EventDto>> GetById(int id)
    {
        var evt = await _db.Events.FindAsync(id);
        if (evt is null) return NotFound(new { message = $"Event {id} not found." });
        return Ok(_mapper.Map<EventDto>(evt));
    }

    // POST api/events
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<EventDto>> Create([FromBody] CreateEventDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var evt = _mapper.Map<Event>(dto);
        _db.Events.Add(evt);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = evt.Id }, _mapper.Map<EventDto>(evt));
    }

    // PUT api/events/{id}
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] CreateEventDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var evt = await _db.Events.FindAsync(id);
        if (evt is null) return NotFound(new { message = $"Event {id} not found." });

        _mapper.Map(dto, evt);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/events/{id}
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var evt = await _db.Events.FindAsync(id);
        if (evt is null) return NotFound(new { message = $"Event {id} not found." });

        _db.Events.Remove(evt);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
