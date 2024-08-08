using BackendChallenge.Application.Rentals;
using BackendChallenge.CrossCutting;
using BackendChallenge.CrossCutting.Endpoints;
using BackendChallenge.CrossCutting.Middlewares;
using BackendChallenge.CrossCutting.Storage;
using Dapper;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BackendChallenge.Application;
public class DependencyInstaller : IDependencyInjectionInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpoints(typeof(DependencyInstaller).Assembly);
        services.AddValidatorsFromAssembly(typeof(DependencyInstaller).Assembly);

        AddInfrastructureServices(services, configuration);
    }

    private static void AddInfrastructureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddExceptionHandler<ExceptionHandlerMiddleware>();
        services.AddProblemDetails();

        services.AddScoped<IStorageService, StorageService>();

        AddDatabase(services, configuration);
        AddCacheServer(services, configuration);
        AddMassTransit(services, configuration);
    }

    private static void AddDatabase(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString =
           configuration.GetConnectionString("Database") ??
           throw new ArgumentNullException(nameof(configuration), "Database connection string is missing");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });

        services.AddScoped<PlanSeed>();

        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
    }

    private static void AddCacheServer(IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis:ConnectionString"];
        });
    }

    private static void AddMassTransit(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            busConfigurator.AddConsumers(typeof(DependencyInstaller).Assembly);
            busConfigurator.UsingRabbitMq((context, busFactoryConfigurator) =>
            {
                busFactoryConfigurator.Host(new Uri(configuration["MessageBroker:Host"]!), h =>
                {
                    h.Username(configuration["MessageBroker:Username"]!);
                    h.Password(configuration["MessageBroker:Password"]!);
                });
                busFactoryConfigurator.ConfigureEndpoints(context);
            });
        });
    }
}

public static class WebApplicationExtensions
{
    public static WebApplication AddApplication(this WebApplication app)
    {
        app.MapEndpoints();
        app.ApplyMigrations();

        return app;
    }

    private static WebApplication ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();

        var seeder = scope.ServiceProvider.GetRequiredService<PlanSeed>();
        seeder.SeedAsync().GetAwaiter().GetResult();

        return app;
    }
}