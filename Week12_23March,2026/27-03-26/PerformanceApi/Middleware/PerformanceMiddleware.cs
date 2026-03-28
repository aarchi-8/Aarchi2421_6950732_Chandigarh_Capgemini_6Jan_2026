using log4net;
namespace PerformanceApi.Middleware
{
    

    public class PerformanceMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ILog log = LogManager.GetLogger(typeof(PerformanceMiddleware));

        public PerformanceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var startTime = DateTime.Now;

            log.Info($"API Started: {context.Request.Path}");

            try
            {
                await _next(context);

                var duration = DateTime.Now - startTime;

                log.Info($"API Completed in {duration.TotalSeconds} sec");

                if (duration.TotalSeconds > 3)
                {
                    log.Warn($"Slow API detected: {duration.TotalSeconds} sec");
                }
            }
            catch (Exception ex)
            {
                log.Error("API failed", ex);
                throw;
            }
        }
    }
}
