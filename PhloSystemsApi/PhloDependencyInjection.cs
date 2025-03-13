using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PhloSystemsApi.Services;
using System.Text;
using System.Text.Json;

namespace PhloSystemsApi
{
    public static class PhloDependencyInjection
    {
        public static IServiceCollection AddPhloSystemDI(this IServiceCollection services, WebApplicationBuilder builder)
        {
            // Add services to the DI container
            services.AddControllers(); // Registers controllers
            _ = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var jwtKey = builder.Configuration["Jwt:Key"];
                    if (string.IsNullOrEmpty(jwtKey))
                    {
                        throw new ArgumentNullException(nameof(jwtKey), "JWT Key cannot be null or empty.");
                    }

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            services.AddAuthorization();
            services.AddHttpClient<IProductService, ProductService>(client =>
            {
                // Base address for the HTTP client
                client.BaseAddress = new Uri("https://pastebin.com/raw/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Configure JSON options
            services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.SerializerOptions.WriteIndented = true;
            });

            return services;
        }
    }
}
