using AutoMapper;
using EventBooking.API.Data;
using EventBooking.API.DTOs;
using EventBooking.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EventBooking.API.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public BookingsController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // GET api/bookings  (returns only the current user's bookings)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetMyBookings()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue("sub")
                     ?? string.Empty;

        var bookings = await _db.Bookings
            .Include(b => b.Event)
            .Where(b => b.UserId == userId)
            .ToListAsync();

        return Ok(_mapper.Map<IEnumerable<BookingDto>>(bookings));
    }

    // POST api/bookings
    [HttpPost]
    public async Task<ActionResult<BookingDto>> Book([FromBody] CreateBookingDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var evt = await _db.Events.FindAsync(dto.EventId);
        if (evt is null)
            return NotFound(new { message = "Event not found." });

        if (evt.AvailableSeats < dto.SeatsBooked)
            return BadRequest(new { message = $"Only {evt.AvailableSeats} seat(s) remaining." });

        var userId   = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? string.Empty;
        var userName = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("name") ?? userId;

        var booking = _mapper.Map<Booking>(dto);
        booking.UserId   = userId;
        booking.UserName = userName;
        booking.BookedAt = DateTime.UtcNow;
        booking.Status   = "Confirmed";

        evt.AvailableSeats -= dto.SeatsBooked;

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        // Reload with Event navigation for mapping
        await _db.Entry(booking).Reference(b => b.Event).LoadAsync();

        return CreatedAtAction(nameof(GetMyBookings), _mapper.Map<BookingDto>(booking));
    }

    // DELETE api/bookings/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? string.Empty;

        var booking = await _db.Bookings
            .Include(b => b.Event)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking is null)
            return NotFound(new { message = "Booking not found." });

        if (booking.UserId != userId)
            return Forbid();

        // Restore seats
        if (booking.Event is not null)
            booking.Event.AvailableSeats += booking.SeatsBooked;

        _db.Bookings.Remove(booking);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
