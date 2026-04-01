using Microsoft.AspNetCore.Mvc;
using EventBooking.API.Data;
using EventBooking.API.Models;
using Microsoft.AspNetCore.Authorization;

namespace EventBooking.API.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingsController(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 BOOK TICKETS
        [HttpPost]
        public IActionResult Book(Booking booking)
        {
            var ev = _context.Events.Find(booking.EventId);

            if (ev == null)
                return NotFound("Event not found");

            if (booking.SeatsBooked > ev.AvailableSeats)
                return BadRequest("Not enough seats available");

            ev.AvailableSeats -= booking.SeatsBooked;

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return Ok("Booking successful");
        }

        // 🔹 CANCEL BOOKING
        [HttpDelete("{id}")]
        public IActionResult Cancel(int id)
        {
            var booking = _context.Bookings.Find(id);

            if (booking == null)
                return NotFound();

            var ev = _context.Events.Find(booking.EventId);

            if (ev != null)
            {
                ev.AvailableSeats += booking.SeatsBooked;
            }

            _context.Bookings.Remove(booking);
            _context.SaveChanges();

            return Ok("Booking cancelled");
        }

        [HttpGet]
        public IActionResult GetBookings()
        {
            var bookings = _context.Bookings.ToList();
            return Ok(bookings);
        }

        [Authorize]
        [HttpDelete("cancel/{id}")]
        public IActionResult CancelBooking(int id)
        {
            var booking = _context.Bookings.Find(id);

            if (booking == null)
                return NotFound();

            var ev = _context.Events.Find(booking.EventId);
            if (ev != null)
            {
                ev.AvailableSeats += booking.SeatsBooked;
            }

            _context.Bookings.Remove(booking);
            _context.SaveChanges();

            return Ok("Booking cancelled");
        }
    }
}