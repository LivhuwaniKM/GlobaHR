using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SHDomain.Helpers;
using System.Text;

namespace StaffHR.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddTransient<TokenHelper>();

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? ""));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = config["Jwt:Issuer"] ?? "",
                        ValidAudience = config["Jwt:Audience"] ?? "",
                        IssuerSigningKey = key,
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
                });

            return services;
        }
    }
}
