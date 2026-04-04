using System;

namespace ProjectBackend.Model.Dto
{
    public class UserActivityDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime LastLoggedIn { get; set; }
        public double LoggedHour { get; set; }
        public DateTime Date { get; set; }
        public int LoginCount { get; set; }
    }

    public class DailyUserActivityDto
    {
        public DateTime Date { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public double TotalLoggedHours { get; set; }
        public double AverageLoggedHours { get; set; }
        public List<UserActivityDto> Users { get; set; } = new();
    }

    public class UserActivitySummaryDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDailyUsers { get; set; }
        public int TotalActiveUsers { get; set; }
        public double TotalLoggedHours { get; set; }
        public double AverageDailyHours { get; set; }
        public List<DailyUserActivityDto> DailyActivities { get; set; } = new();
    }
}
