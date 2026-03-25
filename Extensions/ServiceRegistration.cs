using Microsoft.Extensions.DependencyInjection;
using projectBackend.Repositories;
using projectBackend.Repositories.Interfaces;
using projectBackend.Services;
using projectBackend.Services.IServices;
using projectBackend.Config.MailKit;  // Add this

namespace projectBackend.Extensions;

public static class ServiceRegistration
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        // Register Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        
        // Register Services
        services.AddScoped<IUserService, UserService>();
        
        // Register Auth Service
        services.AddScoped<IAuthService, AuthService>();

        // Register the mail infrastructure (initialization)
        services.AddSingleton<MailService>(); // Singleton because it holds connection
            
        // Register the business logic service
        services.AddScoped<IEmailService, EmailService>();
        
        return services;
    }
}