using Microsoft.AspNetCore.Mvc;
using HotelBookingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelBookingApp.Controllers
{
    public class BookingController : Controller
    {
        public static List<Booking> bookings = new List<Booking>();
        public static List<Customer> customers = new List<Customer>();

        public IActionResult Create(int roomId)
        {
            ViewBag.RoomId = roomId;
            return View();
        }

        [HttpPost]
        public IActionResult Create(Booking booking, string customerName, string email)
        {
            var customer = new Customer
            {
                Id = customers.Count + 1,
                Name = customerName,
                Email = email
            };

            customers.Add(customer);

            booking.Id = bookings.Count + 1;
            booking.CustomerId = customer.Id;

            bookings.Add(booking);

            TempData["success"] = "Booking Confirmed!";
            return RedirectToAction("Index", "Rooms");
        }
    }
}