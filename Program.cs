using projectBackend.Config.Firebase;
using projectBackend.Extensions;  // ← This must be present
using projectBackend.Middleware;
using projectBackend.Repositories.Interfaces;
using projectBackend.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

// Register Swagger
builder.Services.RegisterSwagger();

// Register CORS
builder.Services.RegisterCors();

// Register JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);  // ← This should now work

// Register Firebase Firestore
builder.Services.RegisterFirebase(builder.Configuration);

// Register Repositories and Services
builder.Services.RegisterRepositories();
builder.Services.RegisterServices();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("ReactApp");

// Add custom middleware
app.UseMiddleware<AdminLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();