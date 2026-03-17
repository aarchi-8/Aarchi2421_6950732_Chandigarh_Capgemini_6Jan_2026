using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeePortal.Filters
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            Console.WriteLine("Exception: " + context.Exception.Message);

            context.Result = new ContentResult
            {
                Content = "Something went wrong. Please try again later.",
                StatusCode = 500
            };

            context.ExceptionHandled = true;
        }
    }
}