using System;

namespace HotelBookingApp.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public int RoomId { get; set; }
        public int CustomerId { get; set; }

        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

        // Navigation Properties
        public Room Room { get; set; }
        public Customer Customer { get; set; }
    }
}