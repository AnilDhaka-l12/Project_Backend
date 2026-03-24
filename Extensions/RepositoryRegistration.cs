using projectBackend.Repositories;
using projectBackend.Repositories.Interfaces;

namespace projectBackend.Extensions;

public static class RepositoryRegistration
{
    /// <summary>
    /// Registers all repositories for dependency injection
    /// </summary>
    public static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        // services.AddScoped<IProductRepository, ProductRepository>();
        // services.AddScoped<IOrderRepository, OrderRepository>();
        
        return services;
    }
}