using CampingCore.Application.Email;
using CampingCore.Application.Abstractions.Authentication;
using CampingCore.Application.Abstractions.Geo;
using CampingCore.Application.Abstractions.Security;
using CampingCore.Domain.Repositories;
using CampingCore.Infrastructure.Authentication;
using CampingCore.Infrastructure.Email;
using CampingCore.Infrastructure.ExternalServices;
using CampingCore.Infrastructure.Persistence;
using CampingCore.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CampingCore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null)));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IUserRepository,        UserRepository>();
        services.AddScoped<IRoleRepository,        RoleRepository>();
        services.AddScoped<IPermissionRepository,  PermissionRepository>();
        services.AddScoped<ICampSiteRepository,    CampSiteRepository>();
        services.AddScoped<IReviewRepository,      ReviewRepository>();
        services.AddScoped<IFavoriteRepository,    FavoriteRepository>();
        services.AddScoped<ITripRepository,        TripRepository>();
        services.AddScoped<ITripCampSiteRepository, TripCampSiteRepository>();

        services.AddHttpClient<IGeoApiService, GeoApiService>(client =>
            client.BaseAddress = new Uri(configuration["GeoApi:BaseUrl"]!));

        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;
        if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
        {
            throw new InvalidOperationException(
                $"Configure a non-empty {JwtSettings.SectionName}:{nameof(JwtSettings.SecretKey)} " +
                "(e.g. appsettings.Development.json, User Secrets, or environment variable JwtSettings__SecretKey). " +
                "HS256 requires at least 32 bytes of key material.");
        }

        var signingKeyBytes = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
        if (signingKeyBytes.Length < 32)
        {
            throw new InvalidOperationException(
                $"{JwtSettings.SectionName}:{nameof(JwtSettings.SecretKey)} must be at least 32 UTF-8 bytes for HS256 (current: {signingKeyBytes.Length}).");
        }

        services.Configure<EmailBrandingOptions>(configuration.GetSection(EmailBrandingOptions.SectionName));
        services.Configure<BrevoSettings>(configuration.GetSection(BrevoSettings.SectionName));
        services.AddHttpClient<IEmailService, BrevoEmailService>(client =>
        {
            client.BaseAddress = new Uri("https://api.brevo.com/");
            client.DefaultRequestHeaders.Add("api-key", configuration["Brevo:ApiKey"]);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddScoped<ITokenService, JwtTokenService>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUserService>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidateLifetime         = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer              = jwtSettings.Issuer,
                    ValidAudience            = jwtSettings.Audience,
                    IssuerSigningKey         = new SymmetricSecurityKey(signingKeyBytes),
                };
            });

        return services;
    }
}
