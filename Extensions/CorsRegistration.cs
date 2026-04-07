namespace ProjectBackend.Extensions;

public static class CorsRegistration
{
    /// <summary>
    /// Registers CORS policy for React frontend
    /// </summary>
    public static IServiceCollection RegisterCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("ReactApp", policy =>
            {
                policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        return services;
    }
}
