using ProductInventory.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IProductService, ProductService>(); // DI registration

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

// Needed for test project to access Program
public partial class Program { }