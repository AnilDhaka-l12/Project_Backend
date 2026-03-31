using projectBackend.Model.Entities;
using projectBackend.Repositories.Interfaces;
using projectBackend.Services.IServices;

namespace projectBackend.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

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
}
