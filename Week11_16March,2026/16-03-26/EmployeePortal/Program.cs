using EmployeePortal.Filters;

namespace EmployeePortal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<LogActionFilter>();
            builder.Services.AddScoped<CustomExceptionFilter>();

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<CustomExceptionFilter>();
            });

            builder.Services.AddSession();

            var app = builder.Build();

            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
