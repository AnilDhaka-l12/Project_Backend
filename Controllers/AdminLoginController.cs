using Microsoft.AspNetCore.Mvc;
using projectBackend.Model.RequestModel;
using projectBackend.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using projectBackend.Attributes;
using FluentValidation;

namespace projectBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminLoginController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AdminLoginController(IAuthService authService, IValidator<LoginRequest> loginValidator)
    {
        _authService = authService;
        _loginValidator = loginValidator;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var validationResult = await _loginValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => char.ToLower(g.Key[0]) + g.Key.Substring(1),
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );
            return BadRequest(errors);
        }

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

    [HttpPost("logout")]
    [CheckJwtBlacklist]
    public async Task<IActionResult> Logout()
    {
        // Get token from Authorization header
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
            return BadRequest(new { message = "Token required" });

        var result = await _authService.LogoutAsync(token);

        if (result)
            return Ok(new { message = "Logged out successfully" });

        return BadRequest(new { message = "Logout failed" });
    }

}
