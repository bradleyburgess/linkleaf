using System.Text;
using LinkLeaf.Api.Options;
using LinkLeaf.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace LinkLeaf.Api.Extensions;

public static class JwtAuthExtensions
{
    public static IServiceCollection AddJwtMiddleware(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IJwtService, JwtService>();
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        var jwtOptions = new JwtOptions();
        configuration.GetSection("Jwt").Bind(jwtOptions);


        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtOptions.Key!)
                ),
                ValidateIssuerSigningKey = true,
            });

        return services;
    }
}
