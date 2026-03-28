using log4net;
namespace SecureOrderApi
{

    public class AuthLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ILog log = LogManager.GetLogger(typeof(AuthLoggingMiddleware));

        public AuthLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == 401)
            {
                log.Warn($"Unauthorized access to {context.Request.Path}");
            }
            else if (context.Response.StatusCode == 403)
            {
                log.Warn($"Forbidden access to {context.Request.Path}");
            }
            else if (context.User.Identity.IsAuthenticated)
            {
                log.Info($"Valid token used for {context.Request.Path}");
            }
        }
    }
}
