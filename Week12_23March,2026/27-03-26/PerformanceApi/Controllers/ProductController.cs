using Microsoft.AspNetCore.Mvc;
using log4net;

namespace PerformanceApi.Controllers
{
   

    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProductController));

        [HttpGet]
        public IActionResult Get()
        {
            var startTime = DateTime.Now;

            log.Info("API Started");

            try
            {
                // Simulate slow API
                Thread.Sleep(5000);

                var result = "Product data";

                var duration = DateTime.Now - startTime;

                log.Info($"API Completed in {duration.TotalSeconds} sec");

                if (duration.TotalSeconds > 3)
                {
                    log.Warn($"Slow API detected: {duration.TotalSeconds} sec");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error("API failed", ex);
                return StatusCode(500);
            }
        }
    }
}
