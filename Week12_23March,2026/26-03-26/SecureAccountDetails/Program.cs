using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SecureAccountDetails.Data;
using SecureAccountDetails.Mapping;
using System;
using System.Text;

namespace SecureAccountDetails
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // ✅ FIX DbContext (use your actual context class)
            builder.Services.AddDbContext<AppDbContext>(opt =>
                opt.UseInMemoryDatabase("TestDB"));

            // ✅ AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // ✅ JWT Key
            var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

            // ✅ Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true, // ✅ IMPORTANT
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });

            builder.Services.AddAuthorization();

            // Swagger / OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // ✅ VERY IMPORTANT ORDER
            app.UseAuthentication();   // ❗ Missing in your code
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}