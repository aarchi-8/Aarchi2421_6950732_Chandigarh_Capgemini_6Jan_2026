using System.ComponentModel.DataAnnotations;

namespace EventBooking.API.Entities;

public class Event
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Event title is required.")]
    [StringLength(150, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    [Required(ErrorMessage = "Location is required.")]
    [StringLength(200)]
    public string Location { get; set; } = string.Empty;

    [Range(1, 10000, ErrorMessage = "Available seats must be between 1 and 10000.")]
    public int AvailableSeats { get; set; }

    public string Category { get; set; } = "General";

    public decimal TicketPrice { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
