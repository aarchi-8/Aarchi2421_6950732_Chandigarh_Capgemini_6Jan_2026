using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeePortal.Filters
{
    public class LogActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.Session.GetString("user") ?? "Guest";

            Console.WriteLine($"Action: {context.ActionDescriptor.DisplayName}");
            Console.WriteLine($"User: {user}");
            Console.WriteLine($"Time: {DateTime.Now}");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine("Action Executed");
        }
    }
}