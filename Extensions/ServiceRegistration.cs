using FluentValidation;
using ProjectBackend.Config.Firebase;
using ProjectBackend.Config.Redis;
using ProjectBackend.Validators;
using ProjectBackend.Services;
using ProjectBackend.Services.IServices;
using ProjectBackend.Config.Backblaze;
using ProjectBackend.Config.Licensing;

namespace ProjectBackend.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAllServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register FluentValidation Validators
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

        // Register Redis
        services.AddRedisServices(configuration);

        // Register Controllers
        services.AddControllers();

        // Register Swagger
        services.RegisterSwagger();

        // Register CORS
        services.RegisterCors();

        // Register JWT Authentication
        services.AddJwtAuthentication(configuration);

        // Register Firebase Firestore
        services.RegisterFirebase(configuration);

        // Register Repositories and Services
        services.RegisterRepositories();
        services.RegisterServices();

        // Register FileLu configuration
        services.RegisterBackblaze(configuration);

        ExcelLicenseConfig.Initialize();

        return services;
    }
}
