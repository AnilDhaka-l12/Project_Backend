using ProjectBackend.Model.Dto;
using ProjectBackend.Model.Entities;

namespace ProjectBackend.Repositories.Interfaces;

public interface IUserRepository
{
    // Existing methods
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetByOrganizationAsync(string organization);
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<User> CreateAsync(User user);
    Task<User?> UpdateAsync(string id, User user);
    Task<bool> DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task<bool> EmailExistsAsync(string email, string? excludeId = null);

    // New paginated methods
    Task<PaginatedResult<User>> GetPaginatedAsync(PaginationParams paginationParams);
    Task<PaginatedResult<User>> GetPaginatedByOrganizationAsync(string organization, PaginationParams paginationParams);
    Task<PaginatedResult<User>> GetPaginatedByActiveStatusAsync(bool isActive, PaginationParams paginationParams);
    Task<PaginatedResult<User>> GetPaginatedWithFiltersAsync(UserQueryParams queryParams);
}
