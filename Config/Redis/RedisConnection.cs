using StackExchange.Redis;
using ProjectBackend.Model.Redis;  // Import from Models

namespace ProjectBackend.Config.Redis;

/// <summary>
/// Redis connection logic - handles creating and configuring the connection
/// </summary>
public static class RedisConnection
{
    /// <summary>
    /// Create and configure Redis connection multiplexer
    /// </summary>
    public static IConnectionMultiplexer CreateConnection(RedisSettings settings, ILogger logger)
    {
        try
        {
            logger.LogInformation("Initializing Redis connection to {ConnectionString}",
                settings.ConnectionString.Split(',')[0]); // Log only host, not password

            var options = ConfigurationOptions.Parse(settings.ConnectionString);

            // Apply settings from our model
            options.AbortOnConnectFail = settings.AbortOnConnectFail;
            options.ConnectTimeout = settings.ConnectTimeout;
            options.SyncTimeout = settings.SyncTimeout;
            options.ConnectRetry = settings.ConnectRetry;

            // SSL configuration
            if (settings.Ssl)
            {
                options.Ssl = true;
                if (!string.IsNullOrEmpty(settings.SslHost))
                {
                    options.SslHost = settings.SslHost;
                }
            }

            // Configure for high availability (if using Sentinel)
            if (!string.IsNullOrEmpty(settings.ServiceName) &&
                settings.SentinelEndpoints?.Any() == true)
            {
                options.ServiceName = settings.ServiceName;
                foreach (var endpoint in settings.SentinelEndpoints)
                {
                    options.EndPoints.Add(endpoint);
                }
                options.TieBreaker = "";
                options.CommandMap = CommandMap.Sentinel;
            }

            // Performance optimizations
            options.Proxy = Proxy.None;
            options.PreserveAsyncOrder = false;

            // Create connection with event handlers
            var connection = ConnectionMultiplexer.Connect(options);

            // Register connection events for monitoring
            connection.ConnectionFailed += (sender, args) =>
            {
                logger.LogWarning(args.Exception,
                    "Redis connection failed to {EndPoint}", args.EndPoint);
            };

            connection.ConnectionRestored += (sender, args) =>
            {
                logger.LogInformation("Redis connection restored to {EndPoint}", args.EndPoint);
            };

            connection.ErrorMessage += (sender, args) =>
            {
                logger.LogError("Redis error: {Message}", args.Message);
            };

            logger.LogInformation("Redis connection established successfully");
            return connection;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to establish Redis connection");
            throw;
        }
    }
}
