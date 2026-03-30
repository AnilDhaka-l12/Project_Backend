namespace projectBackend.Services.IServices;

public interface IAuthService
{
    Task<string> LoginAsync(string username, string password);
    Task<bool> LogoutAsync(string token);
}