using Company___Employees.Models;
using Microsoft.EntityFrameworkCore;

namespace Company___Employees
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Companies}/{action=Index}/{id?}")
                .WithStaticAssets();
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();

                if (!context.Companies.Any())
                {
                    var c1 = new Company
                    {
                        Name = "Microsoft",
                        Location = "USA",
                        Employees = new List<Employee>
            {
                new Employee { Name = "John", Position = "Developer" },
                new Employee { Name = "Sara", Position = "Manager" }
            }
                    };

                    var c2 = new Company
                    {
                        Name = "Google",
                        Location = "USA",
                        Employees = new List<Employee>
            {
                new Employee { Name = "Alex", Position = "Engineer" },
                new Employee { Name = "Emma", Position = "Designer" }
            }
                    };

                    context.Companies.AddRange(c1, c2);
                    context.SaveChanges();
                }
            }

            app.Run();
        }
    }
}
