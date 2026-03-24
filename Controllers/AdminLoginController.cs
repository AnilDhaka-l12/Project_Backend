using Microsoft.AspNetCore.Mvc;
using projectBackend.Model.RequestModel;
using projectBackend.Services.IServices;

namespace projectBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminLoginController : ControllerBase
{
    private readonly IAuthService _authService;
    
    public AdminLoginController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        try
        {
            var token = await _authService.LoginAsync(request.Username, request.Password);
            
            if (string.IsNullOrEmpty(token))
            {
                return StatusCode(500, new { message = "Token generation failed" });
            }
            
            Console.WriteLine($"Token from controller: {token}");
            
            // Return token in a proper JSON object
            return Ok(new { token = token, message = "Login successful" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in login: {ex.Message}");
            return StatusCode(500, new { message = $"Error: {ex.Message}" });
        }
    }
}