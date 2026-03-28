using log4net;
namespace SecureOrderApi.Middleware
{
    

    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ILog log = LogManager.GetLogger(typeof(RequestLoggingMiddleware));

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            log.Info($"Request: {context.Request.Method} {context.Request.Path}");

            await _next(context);
        }
    }
}
