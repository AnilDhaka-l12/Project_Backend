using Microsoft.Extensions.DependencyInjection;
using projectBackend.Repositories;
using projectBackend.Repositories.Interfaces;
using projectBackend.Services;
using projectBackend.Services.IServices;
using projectBackend.Config.MailKit;
using FluentValidation;
using projectBackend.Validators;
using projectBackend.Model.RequestModel;

namespace projectBackend.Extensions;

public static class RepositoryRegistration
{
    // Add this missing method
    public static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        // Register Repositories only
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        // Register FluentValidation Validators
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UserRequestModelValidator>();

        // Register Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<MailService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IRedisCacheService, RedisCacheService>();

        return services;
    }
}
