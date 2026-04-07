using ProjectBackend.Model.Dto;
using ProjectBackend.Model.Entities;
using ProjectBackend.Repositories.Interfaces;
using ProjectBackend.Services.IServices;

namespace ProjectBackend.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // Existing methods
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<User?> GetUserByIdAsync(string id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task<IEnumerable<User>> GetUsersByOrganizationAsync(string organization)
    {
        return await _userRepository.GetByOrganizationAsync(organization);
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _userRepository.GetActiveUsersAsync();
    }

    public async Task<User> CreateUserAsync(User user)
    {
        if (await _userRepository.EmailExistsAsync(user.Email))
        {
            throw new InvalidOperationException($"User with email '{user.Email}' already exists");
        }

        return await _userRepository.CreateAsync(user);
    }

    public async Task<User?> UpdateUserAsync(string id, User user)
    {
        if (await _userRepository.EmailExistsAsync(user.Email, id))
        {
            throw new InvalidOperationException($"Email '{user.Email}' is already used by another user");
        }

        return await _userRepository.UpdateAsync(id, user);
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        return await _userRepository.DeleteAsync(id);
    }

    // New paginated methods
    public async Task<PaginatedResult<User>> GetUsersPaginatedAsync(PaginationParams paginationParams)
    {
        return await _userRepository.GetPaginatedAsync(paginationParams);
    }

    public async Task<PaginatedResult<User>> GetUsersByOrganizationPaginatedAsync(string organization, PaginationParams paginationParams)
    {
        return await _userRepository.GetPaginatedByOrganizationAsync(organization, paginationParams);
    }

    public async Task<PaginatedResult<User>> GetActiveUsersPaginatedAsync(PaginationParams paginationParams)
    {
        return await _userRepository.GetPaginatedByActiveStatusAsync(true, paginationParams);
    }

    public async Task<PaginatedResult<User>> GetUsersWithFiltersAsync(UserQueryParams queryParams)
    {
        return await _userRepository.GetPaginatedWithFiltersAsync(queryParams);
    }
}