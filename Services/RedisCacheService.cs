using StackExchange.Redis;
using projectBackend.Config.Redis;
using projectBackend.Services.IServices;

namespace projectBackend.Services;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _database;
    private readonly ILogger<RedisCacheService> _logger;
    private const string TOKEN_PREFIX = "jwt:token:";

    public RedisCacheService(IRedisDatabase redisDatabase, ILogger<RedisCacheService> logger)
    {
        _database = redisDatabase.Database;
        _logger = logger;
    }

    public async Task<bool> StoreTokenAsync(string token, TimeSpan expiry)
    {
        try
        {
            var key = $"{TOKEN_PREFIX}{token}";
            return await _database.StringSetAsync(key, "1", expiry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store token");
            return false;
        }
    }

    public async Task<bool> IsTokenValidAsync(string token)
    {
        try
        {
            var key = $"{TOKEN_PREFIX}{token}";
            return await _database.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check token validity");
            return false;
        }
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        try
        {
            var key = $"{TOKEN_PREFIX}{token}";
            return await _database.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to revoke token");
            return false;
        }
    }
}