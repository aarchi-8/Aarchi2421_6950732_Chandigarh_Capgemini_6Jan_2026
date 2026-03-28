using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using log4net;

namespace SecureOrderApi.Controllers
{
    

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(OrdersController));

        [HttpGet]
        public IActionResult GetOrders()
        {
            log.Info("GET /api/orders called");
            return Ok("Orders data");
        }
    }
}
