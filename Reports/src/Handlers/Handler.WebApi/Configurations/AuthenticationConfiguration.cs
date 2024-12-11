using System.Security.Claims;
using Handler.WebApi.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Handler.WebApi.Configurations;

public static class AuthenticationConfiguration
{
    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtSetting = configuration.GetSection("Jwt").Get<JwtSettings>();

                if (jwtSetting is null)
                {
                    throw new Exception("Jwt settings not provided");
                }

                options.Authority = jwtSetting.Authority;
                options.Audience = jwtSetting.Audience;
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSetting.Authority,
                    ValidateAudience = false,
                    ValidateLifetime = true
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;

                        var scopesClaim = claimsIdentity?.FindAll("scopes");

                        if (scopesClaim == null)
                        {
                            return Task.CompletedTask;
                        }

                        foreach (var role in scopesClaim.ToList())
                        {
                            claimsIdentity?.AddClaim(new Claim(ClaimTypes.Role, role.Value.Trim()));
                        }

                        return Task.CompletedTask;
                    }
                };
            });
    }
}