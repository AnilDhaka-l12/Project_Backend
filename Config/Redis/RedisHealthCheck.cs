using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace projectBackend.Config.Redis;

/// <summary>
/// Redis health check logic
/// </summary>
public class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisHealthCheck> _logger;

    public RedisHealthCheck(IConnectionMultiplexer redis, ILogger<RedisHealthCheck> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var pong = await db.PingAsync();

            var data = new Dictionary<string, object>
            {
                { "Ping", $"{pong.TotalMilliseconds:F2}ms" },
                { "IsConnected", _redis.IsConnected },
                { "OperationCount", _redis.GetCounters().TotalOutstanding },
                { "EndpointCount", _redis.GetEndPoints().Length }
            };

            return HealthCheckResult.Healthy("Redis is operational", data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Redis health check failed");
            return HealthCheckResult.Unhealthy("Redis is unavailable", ex);
        }
    }
}
