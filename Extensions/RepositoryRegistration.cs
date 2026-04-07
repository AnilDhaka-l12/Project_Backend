using Microsoft.Extensions.DependencyInjection;
using ProjectBackend.Repositories;
using ProjectBackend.Repositories.Interfaces;
using ProjectBackend.Services;
using ProjectBackend.Services.IServices;
using ProjectBackend.Config.MailKit;
using FluentValidation;
using ProjectBackend.Validators;
using ProjectBackend.Model.RequestModel;

namespace ProjectBackend.Extensions;

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
        // Register FluentValidclearation Validators
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UserRequestModelValidator>();

        // Register Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<MailService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IRedisCacheService, RedisCacheService>();
        services.AddScoped<IUserExportService, UserExportService>();
        services.AddScoped<IUserActivityRepository, UserActivityRepository>();
        services.AddScoped<IUserActivityService, UserActivityService>();
        services.AddScoped<IFileDownloadService, FileDownloadService>();

        return services;
    }
}
