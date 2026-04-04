using ProjectBackend.Services.IServices;

namespace ProjectBackend.Middlewares;

public class JwtBlacklistMiddleware
{
    private readonly RequestDelegate _next;

    public JwtBlacklistMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IRedisCacheService redisCache)
    {
        var endpoint = context.GetEndpoint();

        // Check if endpoint has our custom attribute
        var shouldCheck = endpoint?.Metadata
            .GetMetadata<Attributes.CheckJwtBlacklistAttribute>() != null;

        if (shouldCheck)
        {
            var token = GetTokenFromRequest(context.Request);

            if (!string.IsNullOrEmpty(token))
            {
                // Check if token is blacklisted in Redis
                var isBlacklisted = await redisCache.IsTokenValidAsync(token);

                if (isBlacklisted)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = "Token has been revoked. Please login again."
                    });
                    return;
                }
            }
        }

        await _next(context);
    }

    private string? GetTokenFromRequest(HttpRequest request)
    {
        return request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
    }
}
