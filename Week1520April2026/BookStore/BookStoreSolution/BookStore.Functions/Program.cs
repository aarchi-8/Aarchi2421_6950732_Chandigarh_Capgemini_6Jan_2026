using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using BookStore.Infrastructure;
using BookStore.Application;
using BookStore.Infrastructure.Data;
using BookStore.Application.Interfaces;
using BookStore.Application.Services;
using BookStore.Infrastructure.Repositories;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        var conn = Environment.GetEnvironmentVariable("DefaultConnection");

        // OPTION 1: Use Extension Methods (Cleaner - Same as BookStore.API)
        services.AddApplicationServices();
        services.AddInfrastructureServices(configuration);

        // OPTION 2: Manual Registration (Alternative approach)
        // Uncomment below if you prefer manual registration:
        /*
        // DB Context
        services.AddDbContext<BookStoreDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")
                ?? Environment.GetEnvironmentVariable("DefaultConnection")));

        // Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IBlobService, BlobService>();

        // Services
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IReportService, ReportService>();
        */
    })
    .Build();

host.Run();
