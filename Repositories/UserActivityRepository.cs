using Google.Cloud.Firestore;
using ProjectBackend.Model.Entities;
using ProjectBackend.Repositories.Interfaces;

namespace ProjectBackend.Repositories
{
    public class UserActivityRepository : IUserActivityRepository
    {
        private readonly FirestoreDb _firestoreDb;
        private const string COLLECTION_NAME = "UserActivities";

        public UserActivityRepository(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        private CollectionReference GetCollection() => _firestoreDb.Collection(COLLECTION_NAME);

        public async Task<UserActivity?> GetByIdAsync(string id)
        {
            var doc = await GetCollection().Document(id).GetSnapshotAsync();
            return doc.Exists ? doc.ConvertTo<UserActivity>() : null;
        }

        public async Task<UserActivity?> GetByUserIdAndDateAsync(string userId, DateTime date)
        {
            var query = GetCollection()
                .WhereEqualTo("UserId", userId)
                .WhereEqualTo("Date", date.Date);

            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.FirstOrDefault()?.ConvertTo<UserActivity>();
        }

        public async Task<IEnumerable<UserActivity>> GetByUserIdAsync(string userId)
        {
            var query = GetCollection().WhereEqualTo("UserId", userId);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<UserActivity>());
        }

        public async Task<IEnumerable<UserActivity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var query = GetCollection()
                .WhereGreaterThanOrEqualTo("Date", startDate.Date)
                .WhereLessThanOrEqualTo("Date", endDate.Date);

            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<UserActivity>());
        }

        public async Task<IEnumerable<UserActivity>> GetByDateAsync(DateTime date)
        {
            var query = GetCollection().WhereEqualTo("Date", date.Date);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<UserActivity>());
        }

        public async Task<UserActivity> CreateAsync(UserActivity userActivity)
        {
            userActivity.Id = Guid.NewGuid().ToString();
            userActivity.UpdatedAt = DateTime.UtcNow;

            var doc = GetCollection().Document(userActivity.Id);
            await doc.SetAsync(userActivity);
            return userActivity;
        }

        public async Task<UserActivity> UpdateAsync(string id, UserActivity userActivity)
        {
            userActivity.UpdatedAt = DateTime.UtcNow;
            var doc = GetCollection().Document(id);
            await doc.SetAsync(userActivity, SetOptions.Overwrite);
            return userActivity;
        }

        public async Task<UserActivity> AddOrUpdateActivityAsync(string userId, double loggedHours)
        {
            var today = DateTime.UtcNow.Date;
            var existingActivity = await GetByUserIdAndDateAsync(userId, today);

            if (existingActivity == null)
            {
                var newActivity = new UserActivity
                {
                    UserId = userId,
                    LastLoggedIn = DateTime.UtcNow,
                    LoggedHour = loggedHours,
                    Date = today,
                    LoginCount = 1,
                    UpdatedAt = DateTime.UtcNow
                };
                return await CreateAsync(newActivity);
            }
            else
            {
                existingActivity.LastLoggedIn = DateTime.UtcNow;
                existingActivity.LoggedHour += loggedHours;
                existingActivity.LoginCount++;
                existingActivity.UpdatedAt = DateTime.UtcNow;
                return await UpdateAsync(existingActivity.Id, existingActivity);
            }
        }

        public async Task<int> GetTotalDailyUsersAsync(DateTime date)
        {
            var activities = await GetByDateAsync(date.Date);
            return activities.Count();
        }

        public async Task<Dictionary<DateTime, int>> GetDailyUserCountAsync(DateTime startDate, DateTime endDate)
        {
            var activities = await GetByDateRangeAsync(startDate, endDate);
            return activities
                .GroupBy(a => a.Date)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task DeleteAsync(string id)
        {
            await GetCollection().Document(id).DeleteAsync();
        }
    }
}
