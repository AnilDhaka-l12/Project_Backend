using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.IdentityModel.Tokens;
using projectBackend.Attributes;
using projectBackend.Model;
using projectBackend.Model.Entities;

namespace projectBackend.Middlewares;

public class AdminLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly FirestoreDb _firestoreDb;
    
    public AdminLoggingMiddleware(RequestDelegate next, IConfiguration configuration, FirestoreDb firestoreDb)
    {
        _next = next;
        _configuration = configuration;
        _firestoreDb = firestoreDb;
    }
    
    public async Task Invoke(HttpContext context)
    {
        // Store original response body stream
        var originalBodyStream = context.Response.Body;
        
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        
        var requestTimestamp = DateTime.UtcNow;
        var endpoint = context.Request.Path.ToString();
        var method = context.Request.Method;
        var payload = await GetRequestBodyAsync(context.Request);
        
        // Process the request
        await _next(context);
        
        var responseStatus = context.Response.StatusCode.ToString();
        
        // Check if this endpoint requires admin logging
        if (ShouldLogAdminCall(context))
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (!string.IsNullOrEmpty(token))
            {
                var adminInfo = GetAdminFromToken(token);
                
                if (adminInfo != null)
                {
                    await LogAdminCallAsync(
                        adminInfo.AdminId,
                        adminInfo.Username,
                        requestTimestamp,
                        endpoint,
                        method,
                        payload,
                        responseStatus
                    );
                }
            }
        }
        
        // IMPORTANT: Reset the stream position before copying back
        responseBody.Seek(0, SeekOrigin.Begin);
        
        // Copy the response body back to the original stream
        await responseBody.CopyToAsync(originalBodyStream);
        
        // Restore the original response body stream
        context.Response.Body = originalBodyStream;
    }
    
    private bool ShouldLogAdminCall(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
            return false;
        
        var controllerActionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
        if (controllerActionDescriptor != null)
        {
            var methodAttribute = controllerActionDescriptor.MethodInfo
                .GetCustomAttribute<AdminAuthorizeAttribute>();
            
            if (methodAttribute != null)
                return true;
            
            var controllerAttribute = controllerActionDescriptor.ControllerTypeInfo
                .GetCustomAttribute<AdminAuthorizeAttribute>();
            
            if (controllerAttribute != null)
                return true;
        }
        
        return false;
    }
    
    private async Task<string> GetRequestBodyAsync(HttpRequest request)
    {
        if (request.ContentLength == null || request.ContentLength == 0)
            return "";
        
        request.EnableBuffering();
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;
        return body;
    }
    
    private AdminInfo? GetAdminFromToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;
        
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not found"));
            
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"] ?? "",
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"] ?? "",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            
            var jwtToken = (JwtSecurityToken)validatedToken;
            
            return new AdminInfo
            {
                AdminId = jwtToken.Claims.FirstOrDefault(c => c.Type == "adminId" || c.Type == ClaimTypes.NameIdentifier)?.Value ?? "",
                Username = jwtToken.Claims.FirstOrDefault(c => c.Type == "username" || c.Type == ClaimTypes.Name)?.Value ?? ""
            };
        }
        catch
        {
            return null;
        }
    }
    
    private async Task LogAdminCallAsync(string adminId, string username, DateTime timestamp, 
        string endpoint, string method, string payload, string responseStatus)
    {
        try
        {
            var adminDoc = _firestoreDb.Collection("Admins").Document(adminId);
            var snapshot = await adminDoc.GetSnapshotAsync();
            
            if (snapshot.Exists)
            {
                var admin = snapshot.ConvertTo<Admin>();
                var apiCallHistory = admin.ApiCallHistory ?? new List<ApiCallRecord>();
                
                apiCallHistory.Add(new ApiCallRecord
                {
                    Timestamp = timestamp,
                    Endpoint = endpoint,
                    Method = method,
                    Payload = payload.Length > 1000 ? payload.Substring(0, 1000) : payload,
                    ResponseStatus = responseStatus
                });
                
                await adminDoc.UpdateAsync(new Dictionary<string, object>
                {
                    { "ApiCallHistory", apiCallHistory }
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to log admin call: {ex.Message}");
        }
    }
}