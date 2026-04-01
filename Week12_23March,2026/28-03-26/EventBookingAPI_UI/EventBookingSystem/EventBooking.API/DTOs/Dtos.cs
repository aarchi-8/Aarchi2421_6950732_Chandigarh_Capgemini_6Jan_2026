using System.ComponentModel.DataAnnotations;
using EventBooking.API.Validation;

namespace EventBooking.API.DTOs;

// ── Event DTOs ──────────────────────────────────────────────────────────────

public class EventDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
    public int AvailableSeats { get; set; }
    public string Category { get; set; } = "General";
    public decimal TicketPrice { get; set; }
}

public class CreateEventDto
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(150, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [FutureDate]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "Location is required.")]
    public string Location { get; set; } = string.Empty;

    [Range(1, 10000)]
    public int AvailableSeats { get; set; }

    public string Category { get; set; } = "General";

    [Range(0, 100000)]
    public decimal TicketPrice { get; set; }
}

// ── Booking DTOs ────────────────────────────────────────────────────────────

public class BookingDto
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public string EventLocation { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public int SeatsBooked { get; set; }
    public DateTime BookedAt { get; set; }
    public string Status { get; set; } = "Confirmed";
}

public class CreateBookingDto
{
    [Required]
    public int EventId { get; set; }

    [Range(1, 100, ErrorMessage = "Seats booked must be between 1 and 100.")]
    public int SeatsBooked { get; set; }
}
