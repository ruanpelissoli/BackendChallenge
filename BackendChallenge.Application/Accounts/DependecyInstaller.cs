﻿using BackendChallenge.CrossCutting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BackendChallenge.Application.Accounts;
public class DependecyInstaller : IDependencyInjectionInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString =
           configuration.GetConnectionString("Database") ??
           throw new ArgumentNullException(nameof(configuration), "Database connection string is missing");

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

        services.AddIdentity<Account, IdentityRole>()
             .AddEntityFrameworkStores<IdentityDbContext>()
             .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"]!,
                ValidAudience = configuration["Jwt:Audience"]!,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
            };
        });

        services.AddScoped<Seed>();

        services.AddAuthorization();
        services.AddAntiforgery();
    }
}

public static class WebApplicationExtensions
{
    public static WebApplication UseIdentity(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAntiforgery();
        app.UseAuthorization();

        var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        dbContext.Database.Migrate();

        var seeder = scope.ServiceProvider.GetRequiredService<Seed>();
        seeder.SeedAsync().GetAwaiter().GetResult();

        return app;
    }
}


