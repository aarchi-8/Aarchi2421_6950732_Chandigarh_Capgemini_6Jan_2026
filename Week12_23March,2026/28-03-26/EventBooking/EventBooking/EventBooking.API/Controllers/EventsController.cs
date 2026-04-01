using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EventBooking.API.Data;
using EventBooking.API.Models;

namespace EventBooking.API.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 GET ALL EVENTS
        [HttpGet]
        public IActionResult GetEvents()
        {
            return Ok(_context.Events.ToList());
        }

        // 🔹 ADD EVENT (ADMIN ONLY)
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult AddEvent(Event e)
        {
            _context.Events.Add(e);
            _context.SaveChanges();
            return Ok("Event added");
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteEvent(int id)
        {
            var ev = _context.Events.Find(id);

            if (ev == null)
                return NotFound("Event not found");

            _context.Events.Remove(ev);
            _context.SaveChanges();

            return Ok("Event deleted");
        }
    }
}