using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using projectBackend.Models.Redis;  // Import from Models

namespace projectBackend.Config.Redis;

/// <summary>
/// Redis configuration and dependency injection setup
/// This contains the LOGIC to register Redis services
/// </summary>
public static class RedisConfig
{
    /// <summary>
    /// Register Redis services with configuration
    /// </summary>
    public static IServiceCollection AddRedisServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. Bind the model from configuration
        var redisSettings = new RedisSettings();
        configuration.GetSection("Redis").Bind(redisSettings);

        // 2. Register the model for DI (so it can be injected elsewhere)
        services.Configure<RedisSettings>(configuration.GetSection("Redis"));

        // 3. Register connection multiplexer (singleton)
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<Program>>();
            var settings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
            return RedisConnection.CreateConnection(settings, logger);
        });

        // 4. Register distributed cache
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisSettings.ConnectionString;
            options.InstanceName = redisSettings.InstanceName;
        });

        // 5. Register health check
        services.AddHealthChecks()
            .AddCheck<RedisHealthCheck>("redis_health");

        // 6. Register database accessor
        services.AddScoped<IRedisDatabase>(sp =>
        {
            var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
            var settings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
            return new RedisDatabase(multiplexer, settings.Database);
        });

        return services;
    }
}

/// <summary>
/// Interface for Redis database access
/// </summary>
public interface IRedisDatabase
{
    IDatabase Database { get; }
    ISubscriber Subscriber { get; }
    IServer GetServer(string endpoint);
}

/// <summary>
/// Implementation of Redis database access
/// </summary>
public class RedisDatabase : IRedisDatabase
{
    private readonly IConnectionMultiplexer _multiplexer;
    private readonly int _databaseId;

    public RedisDatabase(IConnectionMultiplexer multiplexer, int databaseId = 0)
    {
        _multiplexer = multiplexer;
        _databaseId = databaseId;
    }

    public IDatabase Database => _multiplexer.GetDatabase(_databaseId);
    public ISubscriber Subscriber => _multiplexer.GetSubscriber();

    public IServer GetServer(string endpoint)
    {
        return _multiplexer.GetServer(endpoint);
    }
}
