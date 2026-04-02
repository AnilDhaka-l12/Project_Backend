using projectBackend.Model.Dto;

namespace projectBackend.Services.IServices
{
    public interface IUserActivityService
    {
        Task<UserActivityDto> TrackUserActivityAsync(string userId, double loggedHours);
        Task<DailyUserActivityDto> GetDailyActivityAsync(DateTime date);
        Task<UserActivitySummaryDto> GetActivitySummaryAsync(DateTime startDate, DateTime endDate);
        Task<List<UserActivityDto>> GetUserActivityHistoryAsync(string userId);
        Task<byte[]> ExportActivityToCsvAsync(DateTime startDate, DateTime endDate);
        Task<byte[]> ExportActivityToExcelAsync(DateTime startDate, DateTime endDate);
    }
}