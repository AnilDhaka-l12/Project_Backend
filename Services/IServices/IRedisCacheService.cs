namespace ProjectBackend.Services.IServices;

public interface IRedisCacheService
{
    Task<bool> StoreTokenAsync(string token, TimeSpan expiry);
    Task<bool> IsTokenValidAsync(string token);
    Task<bool> RevokeTokenAsync(string token);
}
