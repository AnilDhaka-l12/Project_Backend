using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;  // ← This is correct for BCrypt.Net-Next
using Google.Cloud.Firestore;
using Microsoft.IdentityModel.Tokens;
using ProjectBackend.Services.IServices;


namespace ProjectBackend.Services;

public class AuthService : IAuthService
{
    private readonly FirestoreDb _firestoreDb;
    private readonly IConfiguration _configuration;
    private readonly IRedisCacheService _redisCache;

    public AuthService(FirestoreDb firestoreDb, IConfiguration configuration, IRedisCacheService redisCache)
    {
        _firestoreDb = firestoreDb;
        _configuration = configuration;
        _redisCache = redisCache;
    }

    public async Task<string> LoginAsync(string username, string password)
    {
        Console.WriteLine($"=== LOGIN ATTEMPT ===");
        Console.WriteLine($"Username: {username}");

        var adminsCollection = _firestoreDb.Collection("Admins");
        var query = adminsCollection.WhereEqualTo("Username", username).Limit(1);
        var snapshot = await query.GetSnapshotAsync();

        Console.WriteLine($"Documents found: {snapshot.Documents.Count}");

        if (snapshot.Documents.Count == 0)
        {
            Console.WriteLine("❌ No admin found");
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var adminDoc = snapshot.Documents[0];
        var storedHash = adminDoc.GetValue<string>("Password");
        Console.WriteLine($"Stored Hash: {storedHash}");

        // Verify password with BCrypt
        bool isValid = BCrypt.Net.BCrypt.Verify(password, storedHash);
        Console.WriteLine($"Password verification result: {isValid}");

        if (!isValid)
        {
            Console.WriteLine("❌ Password verification failed");
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        Console.WriteLine("✅ Login successful!");

        var role = adminDoc.GetValue<string>("Role") ?? "Admin";
        var adminId = adminDoc.Id;
        var adminUsername = adminDoc.GetValue<string>("Username");

        Console.WriteLine($"Generating token for: adminId={adminId}, username={adminUsername}, role={role}");

        var token = GenerateJwtToken(adminId, adminUsername, role);

        Console.WriteLine($"Token generated successfully! Token length: {token?.Length ?? 0}");
        Console.WriteLine($"Token: {token}");
        Console.WriteLine($"About to return token...");

        return token;
    }

    public async Task<bool> LogoutAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var expiry = jwtToken.ValidTo;
            var timeRemaining = expiry - DateTime.UtcNow;

            if (timeRemaining > TimeSpan.Zero)
            {
                await _redisCache.StoreTokenAsync(token, timeRemaining);
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }


    private string GenerateJwtToken(string adminId, string username, string role)
    {
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not found");

        Console.WriteLine($"=== JWT KEY DEBUG ===");
        Console.WriteLine($"Key value: {key}");
        Console.WriteLine($"Key length: {key.Length}");
        Console.WriteLine($"===================");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, adminId),
        new Claim(ClaimTypes.Name, username),
        new Claim(ClaimTypes.Role, role),
        new Claim("username", username),
        new Claim("adminId", adminId),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

        var issuer = _configuration["Jwt:Issuer"] ?? "http://localhost:5082";
        var audience = _configuration["Jwt:Audience"] ?? "http://localhost:5082";

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        Console.WriteLine($"Token string created: {(string.IsNullOrEmpty(tokenString) ? "NULL" : "OK")}");

        return tokenString;
    }
}
