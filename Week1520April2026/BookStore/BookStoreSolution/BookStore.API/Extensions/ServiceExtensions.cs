using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
namespace BookStore.API.Extensions;
public static class ServiceExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(o => { o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; })
        .AddJwtBearer(o => { o.TokenValidationParameters = new TokenValidationParameters { ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true, ValidIssuer = config["Jwt:Issuer"], ValidAudience = config["Jwt:Audience"], IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)) }; });
        return services;
    }
    public static IServiceCollection AddApiVersioningConfig(this IServiceCollection services)
    {
        services.AddApiVersioning(o => { o.DefaultApiVersion = new ApiVersion(1, 0); o.AssumeDefaultVersionWhenUnspecified = true; o.ReportApiVersions = true; o.ApiVersionReader = new UrlSegmentApiVersionReader(); })
        .AddApiExplorer(o => { o.GroupNameFormat = "'v'VVV"; o.SubstituteApiVersionInUrl = true; });
        return services;
    }
    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddSwaggerGen(o => { o.SwaggerDoc("v1", new OpenApiInfo { Title = "BookStore API", Version = "v1" }); o.SwaggerDoc("v2", new OpenApiInfo { Title = "BookStore API", Version = "v2" });
            o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Name = "Authorization", Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT", In = ParameterLocation.Header, Description = "Enter JWT token" });
            o.AddSecurityRequirement(new OpenApiSecurityRequirement { { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() } }); });
        return services;
    }
}