using Microsoft.AspNetCore.Mvc;
using HotelBookingApp.Models;
using System.Collections.Generic;

namespace HotelBookingApp.Controllers
{
    public class RoomsController : Controller
    {
        public static List<Room> rooms = new List<Room>()
        {
            new Room { Id = 1, RoomNumber = "101", Type = "Deluxe", Price = 3000, ImageUrl = "https://via.placeholder.com/400" },
            new Room { Id = 2, RoomNumber = "102", Type = "Suite", Price = 5000, ImageUrl = "https://via.placeholder.com/400" }
        };

        public IActionResult Index()
        {
            return View(rooms);
        }
    }
}