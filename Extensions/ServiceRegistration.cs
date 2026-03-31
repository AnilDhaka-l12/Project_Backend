using FluentValidation;
using projectBackend.Config.Firebase;
using projectBackend.Config.Redis;
using projectBackend.Validators;

namespace projectBackend.Extensions;

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

        return services;
    }
}
