using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nexo.Application.Common.Interfaces;
using Nexo.Infrastructure.Data;
using Nexo.Infrastructure.Repositories;
using Nexo.Infrastructure.Services;
using StackExchange.Redis;

namespace Nexo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // PostgreSQL
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("Postgres"),
                npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Redis (opcional - no falla si no está disponible)
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            try
            {
                var redisConnectionString = configuration.GetConnectionString("Redis");
                var options = ConfigurationOptions.Parse(redisConnectionString);
                options.AbortOnConnectFail = false;
                options.ConnectTimeout = 3000;
                options.ConnectRetry = 1;

                var logger = sp.GetRequiredService<ILogger<IConnectionMultiplexer>>();
                var connection = ConnectionMultiplexer.Connect(options);

                connection.ConnectionFailed += (sender, args) =>
                {
                    logger.LogWarning($"Redis connection failed: {args.Exception?.Message}");
                };

                connection.ConnectionRestored += (sender, args) =>
                {
                    logger.LogInformation("Redis connection restored");
                };

                logger.LogInformation("Redis configured successfully");
                return connection;
            }
            catch (Exception ex)
            {
                var logger = sp.GetRequiredService<ILogger<IConnectionMultiplexer>>();
                logger.LogWarning(ex, "Redis not available, using in-memory fallback");
                return null!;
            }
        });

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();

        // Services
        services.AddScoped<IStorageService, StorageService>();

        // EventBus condicional (corregido)
        services.AddScoped<IEventBus>(sp =>
        {
            var redis = sp.GetRequiredService<IConnectionMultiplexer>();
            if (redis != null && redis.IsConnected)
            {
                return new EventBusService(redis);
            }

            // Intentar obtener logger, si no existe usar null
            var logger = sp.GetService<ILogger<InMemoryEventBusService>>();
            return new InMemoryEventBusService(logger);
        });

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, PasswordHasherService>();

        return services;
    }
}