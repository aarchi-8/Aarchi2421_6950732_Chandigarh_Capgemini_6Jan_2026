using OrderProcessing.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IOrderService, OrderService>(); // ← Register service

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();