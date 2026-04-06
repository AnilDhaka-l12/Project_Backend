using Microsoft.AspNetCore.Mvc;
using ProjectBackend.Services.IServices;  // Changed from .Interfaces

namespace ProjectBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RedisTestController : ControllerBase
{
    private readonly IRedisCacheService _redisCache;

    public RedisTestController(IRedisCacheService redisCache)
    {
        _redisCache = redisCache;
    }

    [HttpPost("store")]
    public async Task<IActionResult> StoreToken([FromBody] StoreTokenRequest request)
    {
        var result = await _redisCache.StoreTokenAsync(request.Token, TimeSpan.FromHours(request.ExpiryHours));

        if (result)
            return Ok(new { Message = "Token stored successfully" });

        return BadRequest(new { Message = "Failed to store token" });
    }

    [HttpGet("check/{token}")]
    public async Task<IActionResult> CheckToken(string token)
    {
        var isValid = await _redisCache.IsTokenValidAsync(token);

        return Ok(new { Token = token, IsValid = isValid });
    }

    [HttpDelete("revoke/{token}")]
    public async Task<IActionResult> RevokeToken(string token)
    {
        var result = await _redisCache.RevokeTokenAsync(token);

        if (result)
            return Ok(new { Message = "Token revoked successfully" });

        return NotFound(new { Message = "Token not found" });
    }
}

public class StoreTokenRequest
{
    public string Token { get; set; } = string.Empty;
    public int ExpiryHours { get; set; } = 2;
}
