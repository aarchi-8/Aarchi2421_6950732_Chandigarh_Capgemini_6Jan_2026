using E_Commerce_Order_Management_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Order_Management_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetCustomers()
        {
            return Ok(_context.Customers.ToList());
        }
    }

}
