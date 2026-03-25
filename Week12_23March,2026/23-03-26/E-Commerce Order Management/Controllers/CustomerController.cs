using E_Commerce_Order_Management.Data;
using E_Commerce_Order_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace E_Commerce_Order_Management.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext Context)
        {
            _context = Context;
        }

        //Get Customer

        public IActionResult Index()
        {
            var customers = _context.Customers
                .Include(c => c.Orders)   // 🔥 IMPORTANT
                .ToList();

            return View(customers);
        }

        //GET Create Customer
        public IActionResult Create()
        {
            return View();
        }

        //POST Create Customer

        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Customers.Add(customer);
                _context.SaveChanges();
                return RedirectToAction("Index");   

            }
            return View(customer);
        }

        //Order History

        public IActionResult Orders(int id)
        {
            var orders = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.ShippingDetail)
                .Where(o => o.CustomerId == id)
                .ToList();

            return View(orders);
        }
    }
}
