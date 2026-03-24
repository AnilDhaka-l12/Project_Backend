using projectBackend.Repositories;
using projectBackend.Repositories.Interfaces;
using projectBackend.Services;
using projectBackend.Services.IServices;

namespace projectBackend.Extensions;

public static class ServiceRegistration
{
    /// <summary>
    /// Registers all services and repositories for dependency injection
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns>IServiceCollection with registered services</returns>
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        // Register Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        
        // Register Services
        services.AddScoped<IUserService, UserService>();
        
        //Register Auth Service
        services.AddScoped<IAuthService, AuthService>();

        // Add more repositories and services here as your project grows
        // services.AddScoped<IProductRepository, ProductRepository>();
        // services.AddScoped<IProductService, ProductService>();
        
        return services;
    }
}