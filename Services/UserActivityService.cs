using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using ProjectBackend.Model.Dto;
using ProjectBackend.Model.Entities;
using ProjectBackend.Repositories.Interfaces;
using ProjectBackend.Services.IServices;

namespace ProjectBackend.Services
{
    public class UserActivityService : IUserActivityService
    {
        private readonly IUserActivityRepository _userActivityRepository;
        private readonly IUserRepository _userRepository;

        public UserActivityService(IUserActivityRepository userActivityRepository, IUserRepository userRepository)
        {
            _userActivityRepository = userActivityRepository;
            _userRepository = userRepository;
        }

        public async Task<UserActivityDto> TrackUserActivityAsync(string userId, double loggedHours)
        {
            var activity = await _userActivityRepository.AddOrUpdateActivityAsync(userId, loggedHours);
            var user = await _userRepository.GetByIdAsync(userId);

            return new UserActivityDto
            {
                UserId = userId,
                UserEmail = user?.Email ?? string.Empty,
                UserName = user?.FullName ?? string.Empty,
                LastLoggedIn = activity.LastLoggedIn,
                LoggedHour = activity.LoggedHour,
                Date = activity.Date,
                LoginCount = activity.LoginCount
            };
        }

        public async Task<DailyUserActivityDto> GetDailyActivityAsync(DateTime date)
        {
            var activities = await _userActivityRepository.GetByDateAsync(date.Date);
            var userActivities = new List<UserActivityDto>();
            double totalHours = 0;
            var activeUsers = 0;

            foreach (var activity in activities)
            {
                var user = await _userRepository.GetByIdAsync(activity.UserId);
                if (user != null && user.IsActive)
                {
                    activeUsers++;
                }

                totalHours += activity.LoggedHour;

                userActivities.Add(new UserActivityDto
                {
                    UserId = activity.UserId,
                    UserEmail = user?.Email ?? string.Empty,
                    UserName = user?.FullName ?? string.Empty,
                    LastLoggedIn = activity.LastLoggedIn,
                    LoggedHour = activity.LoggedHour,
                    Date = activity.Date,
                    LoginCount = activity.LoginCount
                });
            }

            return new DailyUserActivityDto
            {
                Date = date.Date,
                TotalUsers = activities.Count(),
                ActiveUsers = activeUsers,
                TotalLoggedHours = totalHours,
                AverageLoggedHours = activities.Any() ? totalHours / activities.Count() : 0,
                Users = userActivities
            };
        }

        public async Task<UserActivitySummaryDto> GetActivitySummaryAsync(DateTime startDate, DateTime endDate)
        {
            var activities = await _userActivityRepository.GetByDateRangeAsync(startDate, endDate);
            var dailyActivities = new List<DailyUserActivityDto>();
            var dailyGroups = activities.GroupBy(a => a.Date);

            var totalUsersSet = new HashSet<string>();
            var totalActiveUsersSet = new HashSet<string>();
            double totalHours = 0;

            foreach (var group in dailyGroups.OrderBy(g => g.Key))
            {
                var dailyTotalUsers = 0;
                var dailyActiveUsers = 0;
                var dailyHours = 0.0;

                foreach (var activity in group)
                {
                    var user = await _userRepository.GetByIdAsync(activity.UserId);
                    dailyTotalUsers++;
                    totalUsersSet.Add(activity.UserId);

                    if (user?.IsActive == true)
                    {
                        dailyActiveUsers++;
                        totalActiveUsersSet.Add(activity.UserId);
                    }

                    dailyHours += activity.LoggedHour;
                    totalHours += activity.LoggedHour;
                }

                dailyActivities.Add(new DailyUserActivityDto
                {
                    Date = group.Key,
                    TotalUsers = dailyTotalUsers,
                    ActiveUsers = dailyActiveUsers,
                    TotalLoggedHours = dailyHours,
                    AverageLoggedHours = dailyTotalUsers > 0 ? dailyHours / dailyTotalUsers : 0,
                    Users = new List<UserActivityDto>()
                });
            }

            return new UserActivitySummaryDto
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalDailyUsers = totalUsersSet.Count,
                TotalActiveUsers = totalActiveUsersSet.Count,
                TotalLoggedHours = totalHours,
                AverageDailyHours = dailyActivities.Any() ? totalHours / dailyActivities.Count : 0,
                DailyActivities = dailyActivities
            };
        }

        public async Task<List<UserActivityDto>> GetUserActivityHistoryAsync(string userId)
        {
            var activities = await _userActivityRepository.GetByUserIdAsync(userId);
            var user = await _userRepository.GetByIdAsync(userId);
            var result = new List<UserActivityDto>();

            foreach (var activity in activities)
            {
                result.Add(new UserActivityDto
                {
                    UserId = activity.UserId,
                    UserEmail = user?.Email ?? string.Empty,
                    UserName = user?.FullName ?? string.Empty,
                    LastLoggedIn = activity.LastLoggedIn,
                    LoggedHour = activity.LoggedHour,
                    Date = activity.Date,
                    LoginCount = activity.LoginCount
                });
            }

            return result.OrderByDescending(a => a.Date).ToList();
        }

        public async Task<byte[]> ExportActivityToCsvAsync(DateTime startDate, DateTime endDate)
        {
            var summary = await GetActivitySummaryAsync(startDate, endDate);
            var csvBuilder = new StringBuilder();

            csvBuilder.AppendLine("Date,Total Users,Active Users,Total Hours,Average Hours");

            foreach (var daily in summary.DailyActivities)
            {
                csvBuilder.AppendLine($"{daily.Date:yyyy-MM-dd},{daily.TotalUsers},{daily.ActiveUsers},{daily.TotalLoggedHours:F2},{daily.AverageLoggedHours:F2}");
            }

            csvBuilder.AppendLine();
            csvBuilder.AppendLine("Summary,,,,,");
            csvBuilder.AppendLine($"Total Unique Users,{summary.TotalDailyUsers},,,,");
            csvBuilder.AppendLine($"Total Active Users,{summary.TotalActiveUsers},,,,");
            csvBuilder.AppendLine($"Total Logged Hours,{summary.TotalLoggedHours:F2},,,,");
            csvBuilder.AppendLine($"Average Daily Hours,{summary.AverageDailyHours:F2},,,,");

            return Encoding.UTF8.GetBytes(csvBuilder.ToString());
        }

        public async Task<byte[]> ExportActivityToExcelAsync(DateTime startDate, DateTime endDate)
        {
            var summary = await GetActivitySummaryAsync(startDate, endDate);

            using var package = new ExcelPackage();

            // Summary Sheet
            var summarySheet = package.Workbook.Worksheets.Add("Summary");
            summarySheet.Cells[1, 1].Value = "User Activity Summary Report";
            summarySheet.Cells[1, 1, 1, 3].Merge = true;
            summarySheet.Cells[1, 1].Style.Font.Size = 16;
            summarySheet.Cells[1, 1].Style.Font.Bold = true;

            summarySheet.Cells[3, 1].Value = "Start Date:";
            summarySheet.Cells[3, 2].Value = summary.StartDate.ToString("yyyy-MM-dd");
            summarySheet.Cells[4, 1].Value = "End Date:";
            summarySheet.Cells[4, 2].Value = summary.EndDate.ToString("yyyy-MM-dd");
            summarySheet.Cells[6, 1].Value = "Total Unique Users:";
            summarySheet.Cells[6, 2].Value = summary.TotalDailyUsers;
            summarySheet.Cells[7, 1].Value = "Total Active Users:";
            summarySheet.Cells[7, 2].Value = summary.TotalActiveUsers;
            summarySheet.Cells[8, 1].Value = "Total Logged Hours:";
            summarySheet.Cells[8, 2].Value = summary.TotalLoggedHours;
            summarySheet.Cells[9, 1].Value = "Average Daily Hours:";
            summarySheet.Cells[9, 2].Value = summary.AverageDailyHours;

            summarySheet.Cells[1, 1, 9, 2].AutoFitColumns();

            // Daily Activity Sheet
            var dailySheet = package.Workbook.Worksheets.Add("Daily Activity");

            var headers = new[] { "Date", "Total Users", "Active Users", "Total Hours", "Average Hours" };
            for (int i = 0; i < headers.Length; i++)
            {
                dailySheet.Cells[1, i + 1].Value = headers[i];
                dailySheet.Cells[1, i + 1].Style.Font.Bold = true;
                dailySheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                dailySheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            }

            int row = 2;
            foreach (var daily in summary.DailyActivities)
            {
                dailySheet.Cells[row, 1].Value = daily.Date.ToString("yyyy-MM-dd");
                dailySheet.Cells[row, 2].Value = daily.TotalUsers;
                dailySheet.Cells[row, 3].Value = daily.ActiveUsers;
                dailySheet.Cells[row, 4].Value = daily.TotalLoggedHours;
                dailySheet.Cells[row, 5].Value = daily.AverageLoggedHours;
                row++;
            }

            dailySheet.Cells[1, 1, row - 1, 5].AutoFitColumns();

            return package.GetAsByteArray();
        }
    }
}
