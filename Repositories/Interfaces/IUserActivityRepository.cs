using ProjectBackend.Model.Entities;

namespace ProjectBackend.Repositories.Interfaces
{
    public interface IUserActivityRepository
    {
        Task<UserActivity?> GetByIdAsync(string id);
        Task<UserActivity?> GetByUserIdAndDateAsync(string userId, DateTime date);
        Task<IEnumerable<UserActivity>> GetByUserIdAsync(string userId);
        Task<IEnumerable<UserActivity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<UserActivity>> GetByDateAsync(DateTime date);
        Task<UserActivity> CreateAsync(UserActivity userActivity);
        Task<UserActivity> UpdateAsync(string id, UserActivity userActivity);
        Task<UserActivity> AddOrUpdateActivityAsync(string userId, double loggedHours);
        Task<int> GetTotalDailyUsersAsync(DateTime date);
        Task<Dictionary<DateTime, int>> GetDailyUserCountAsync(DateTime startDate, DateTime endDate);
        Task DeleteAsync(string id);
    }
}
