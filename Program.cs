using projectBackend.Extensions;
using projectBackend.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Register all services
builder.Services.AddAllServices(builder.Configuration);

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("ReactApp");

// Add custom middleware
app.UseMiddleware<AdminLoggingMiddleware>();
app.UseMiddleware<JwtBlacklistMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();