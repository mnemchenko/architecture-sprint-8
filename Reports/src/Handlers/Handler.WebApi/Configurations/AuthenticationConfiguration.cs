using System.Security.Claims;
using System.Text.Json;
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
                    ValidateIssuer = false,
                    ValidIssuer = jwtSetting.Authority,
                    ValidateAudience = false,
                    ValidateLifetime = true
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                        if (claimsIdentity == null) return Task.CompletedTask;

                        // Extract realm_access.roles from the token
                        var realmAccessClaim = claimsIdentity.FindFirst("realm_access");
                        if (realmAccessClaim != null && !string.IsNullOrWhiteSpace(realmAccessClaim.Value))
                        {
                            try
                            {
                                using var jsonDoc = JsonDocument.Parse(realmAccessClaim.Value);
                                if (jsonDoc.RootElement.TryGetProperty("roles", out var rolesElement) && rolesElement.ValueKind == JsonValueKind.Array)
                                {
                                    foreach (var role in rolesElement.EnumerateArray())
                                    {
                                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.GetString() ?? string.Empty));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Failed to parse realm_access.roles: {ex.Message}");
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
            });
    }
}