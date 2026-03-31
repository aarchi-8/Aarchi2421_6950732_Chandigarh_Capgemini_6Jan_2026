using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public IActionResult GetEvents()
        {
            return Ok(_context.Events.ToList());
        }
        [HttpPost]
        public IActionResult AddEvent(EventBooking.API.DTOs.EventDto dto)
{
    var ev = new EventBooking.API.Models.Event
    {
        Title = dto.Title,
        Description = dto.Description,
        Date = dto.Date,
        Location = dto.Location,
        AvailableSeats = dto.AvailableSeats
    };

    _context.Events.Add(ev);
    _context.SaveChanges();

    return Ok("Event added successfully");
}
    }
}
    
