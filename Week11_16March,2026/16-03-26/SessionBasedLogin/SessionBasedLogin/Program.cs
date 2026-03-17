namespace SessionBasedLogin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllersWithViews();

            // Add Session service
            builder.Services.AddSession();

            var app = builder.Build();

            app.UseStaticFiles();

            app.UseRouting();

            // Enable Session middleware
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}