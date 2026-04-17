using Google.Cloud.Firestore;
using ProjectBackend.Model.Dto;
using ProjectBackend.Model.Entities;
using ProjectBackend.Repositories.Interfaces;

namespace ProjectBackend.Repositories;

public class UserRepository : IUserRepository
{
    private readonly FirestoreDb _firestoreDb;
    private readonly CollectionReference _usersCollection;

    public UserRepository(FirestoreDb firestoreDb)
    {
        _firestoreDb = firestoreDb;
        _usersCollection = _firestoreDb.Collection("Users");
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var snapshot = await _usersCollection.OrderByDescending("CreatedAt").GetSnapshotAsync();
        return snapshot.Documents.Select(doc =>
        {
            var user = doc.ConvertTo<User>();
            user.Id = doc.Id;
            return user;
        });
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        var docRef = _usersCollection.Document(id);
        var snapshot = await docRef.GetSnapshotAsync();

        if (!snapshot.Exists)
            return null;

        var user = snapshot.ConvertTo<User>();
        user.Id = snapshot.Id;
        return user;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var query = _usersCollection.WhereEqualTo("Email", email).Limit(1);
        var snapshot = await query.GetSnapshotAsync();

        if (snapshot.Documents.Count == 0)
            return null;

        var user = snapshot.Documents[0].ConvertTo<User>();
        user.Id = snapshot.Documents[0].Id;
        return user;
    }

    public async Task<IEnumerable<User>> GetByOrganizationAsync(string organization)
    {
        var query = _usersCollection.WhereEqualTo("Organization", organization)
                                     .OrderBy("Name");
        var snapshot = await query.GetSnapshotAsync();

        return snapshot.Documents.Select(doc =>
        {
            var user = doc.ConvertTo<User>();
            user.Id = doc.Id;
            return user;
        });
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        var query = _usersCollection.WhereEqualTo("IsActive", true)
                                     .OrderByDescending("CreatedAt");
        var snapshot = await query.GetSnapshotAsync();

        return snapshot.Documents.Select(doc =>
        {
            var user = doc.ConvertTo<User>();
            user.Id = doc.Id;
            return user;
        });
    }

    // NEW: Paginated methods for Firestore
    public async Task<PaginatedResult<User>> GetPaginatedAsync(PaginationParams paginationParams)
    {
        // Start with base query
        Query query = _usersCollection;

        // Apply filters
        if (!string.IsNullOrEmpty(paginationParams.SearchTerm))
        {
            // Firestore doesn't support text search natively
            // You can filter by email or use a search index
            query = query.WhereEqualTo("Email", paginationParams.SearchTerm);
        }

        if (!string.IsNullOrEmpty(paginationParams.Organization))
        {
            query = query.WhereEqualTo("Organization", paginationParams.Organization);
        }

        if (paginationParams.IsActive.HasValue)
        {
            query = query.WhereEqualTo("IsActive", paginationParams.IsActive.Value);
        }

        // Get total count
        var countSnapshot = await query.GetSnapshotAsync();
        var totalCount = countSnapshot.Documents.Count;

        // Apply sorting and pagination
        var orderedQuery = query.OrderByDescending("CreatedAt");

        var paginatedQuery = orderedQuery
            .Offset((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Limit(paginationParams.PageSize);

        var snapshot = await paginatedQuery.GetSnapshotAsync();
        var items = snapshot.Documents.Select(doc =>
        {
            var user = doc.ConvertTo<User>();
            user.Id = doc.Id;
            return user;
        });

        return new PaginatedResult<User>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize
        };
    }

    public async Task<PaginatedResult<User>> GetPaginatedByOrganizationAsync(string organization, PaginationParams paginationParams)
    {
        var query = _usersCollection.WhereEqualTo("Organization", organization);

        // Get total count
        var countSnapshot = await query.GetSnapshotAsync();
        var totalCount = countSnapshot.Documents.Count;

        // Apply sorting and pagination
        var paginatedQuery = query
            .OrderByDescending("CreatedAt")
            .Offset((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Limit(paginationParams.PageSize);

        var snapshot = await paginatedQuery.GetSnapshotAsync();
        var items = snapshot.Documents.Select(doc =>
        {
            var user = doc.ConvertTo<User>();
            user.Id = doc.Id;
            return user;
        });

        return new PaginatedResult<User>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize
        };
    }

    public async Task<PaginatedResult<User>> GetPaginatedByActiveStatusAsync(bool isActive, PaginationParams paginationParams)
    {
        var query = _usersCollection.WhereEqualTo("IsActive", isActive);

        // Get total count
        var countSnapshot = await query.GetSnapshotAsync();
        var totalCount = countSnapshot.Documents.Count;

        // Apply sorting and pagination
        var paginatedQuery = query
            .OrderByDescending("CreatedAt")
            .Offset((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Limit(paginationParams.PageSize);

        var snapshot = await paginatedQuery.GetSnapshotAsync();
        var items = snapshot.Documents.Select(doc =>
        {
            var user = doc.ConvertTo<User>();
            user.Id = doc.Id;
            return user;
        });

        return new PaginatedResult<User>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize
        };
    }

    public async Task<PaginatedResult<User>> GetPaginatedWithFiltersAsync(UserQueryParams queryParams)
    {
        Query query = _usersCollection;

        // Apply filters
        if (!string.IsNullOrEmpty(queryParams.Organization))
        {
            query = query.WhereEqualTo("Organization", queryParams.Organization);
        }

        if (queryParams.IsActive.HasValue)
        {
            query = query.WhereEqualTo("IsActive", queryParams.IsActive.Value);
        }

        if (!string.IsNullOrEmpty(queryParams.Email))
        {
            query = query.WhereEqualTo("Email", queryParams.Email);
        }

        // Get total count
        var countSnapshot = await query.GetSnapshotAsync();
        var totalCount = countSnapshot.Documents.Count;

        // Apply sorting and pagination
        var orderedQuery = query.OrderByDescending("CreatedAt");

        var paginatedQuery = orderedQuery
            .Offset((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Limit(queryParams.PageSize);

        var snapshot = await paginatedQuery.GetSnapshotAsync();
        var items = snapshot.Documents.Select(doc =>
        {
            var user = doc.ConvertTo<User>();
            user.Id = doc.Id;
            return user;
        });

        return new PaginatedResult<User>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = queryParams.PageNumber,
            PageSize = queryParams.PageSize
        };
    }

    public async Task<User> CreateAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.IsActive = true;

        var document = await _usersCollection.AddAsync(new
        {
            user.Name,
            user.Surname,
            user.Email,
            user.Occupation,
            user.Organization,
            user.CreatedAt,
            user.IsActive
        });

        user.Id = document.Id;
        return user;
    }

    public async Task<User?> UpdateAsync(string id, User user)
    {
        var docRef = _usersCollection.Document(id);
        var snapshot = await docRef.GetSnapshotAsync();

        if (!snapshot.Exists)
            return null;

        var updates = new Dictionary<string, object>
        {
            { "Name", user.Name },
            { "Surname", user.Surname },
            { "Email", user.Email },
            { "Occupation", user.Occupation },
            { "Organization", user.Organization },
            { "IsActive", user.IsActive },
            { "UpdatedAt", DateTime.UtcNow }
        };

        await docRef.UpdateAsync(updates);

        user.Id = id;
        user.UpdatedAt = DateTime.UtcNow;
        return user;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var docRef = _usersCollection.Document(id);
        await docRef.DeleteAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(string id)
    {
        var docRef = _usersCollection.Document(id);
        var snapshot = await docRef.GetSnapshotAsync();
        return snapshot.Exists;
    }

    public async Task<bool> EmailExistsAsync(string email, string? excludeId = null)
    {
        var query = _usersCollection.WhereEqualTo("Email", email);
        var snapshot = await query.GetSnapshotAsync();

        if (snapshot.Documents.Count == 0)
            return false;

        if (excludeId != null && snapshot.Documents[0].Id == excludeId)
            return false;

        return true;
    }


}
