using projectBackend.Model.Entities;

namespace projectBackend.Services.IServices;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(string id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<IEnumerable<User>> GetUsersByOrganizationAsync(string organization);
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<User> CreateUserAsync(User user);
    Task<User?> UpdateUserAsync(string id, User user);
    Task<bool> DeleteUserAsync(string id);
}