namespace ProjectBackend.Extensions;

public static class CorsRegistration
{
    public static IServiceCollection RegisterCors(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        var frontendUrl = configuration["Cors:FrontendUrl"];
        var adminUrl = configuration["Cors:AdminUrl"];
        var swaggerUrl = configuration["Cors:SwaggerURL"];

        var origins = new List<string>();

        if (!string.IsNullOrWhiteSpace(frontendUrl))
            origins.Add(frontendUrl);

        if (!string.IsNullOrWhiteSpace(adminUrl))
            origins.Add(adminUrl);

        if (!string.IsNullOrWhiteSpace(swaggerUrl))
            origins.Add(swaggerUrl);

        services.AddCors(options =>
        {
            options.AddPolicy("ReactApp", policy =>
            {
                policy.WithOrigins(origins.ToArray())
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        return services;
    }
}