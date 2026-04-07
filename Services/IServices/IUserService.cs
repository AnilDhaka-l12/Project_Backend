using ProjectBackend.Model.Dto;
using ProjectBackend.Model.Entities;

namespace ProjectBackend.Services.IServices;

public interface IUserService
{
    // Existing methods
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(string id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<IEnumerable<User>> GetUsersByOrganizationAsync(string organization);
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<User> CreateUserAsync(User user);
    Task<User?> UpdateUserAsync(string id, User user);
    Task<bool> DeleteUserAsync(string id);
    
    // New paginated methods
    Task<PaginatedResult<User>> GetUsersPaginatedAsync(PaginationParams paginationParams);
    Task<PaginatedResult<User>> GetUsersByOrganizationPaginatedAsync(string organization, PaginationParams paginationParams);
    Task<PaginatedResult<User>> GetActiveUsersPaginatedAsync(PaginationParams paginationParams);
    Task<PaginatedResult<User>> GetUsersWithFiltersAsync(UserQueryParams queryParams);
}