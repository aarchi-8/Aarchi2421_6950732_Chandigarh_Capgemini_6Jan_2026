using Microsoft.AspNetCore.Mvc.Filters;

namespace StudentManagementSystem.Filters
{
    public class LogActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine($"Action: {context.ActionDescriptor.DisplayName}");
            Console.WriteLine($"Time: {DateTime.Now}");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine("Action Executed");
        }
    }
}