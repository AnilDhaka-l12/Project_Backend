using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using projectBackend.Attributes;
using projectBackend.Model.Entities;
using projectBackend.Model.RequestModel;
using projectBackend.Services.IServices;

namespace projectBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet]
    [AdminAuthorize]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }
    
    [HttpGet("{id}")]
    [AdminAuthorize]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound(new { message = $"User with ID {id} not found" });
        
        return Ok(user);
    }
    
    [HttpGet("email/{email}")]
    [AdminAuthorize]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var user = await _userService.GetUserByEmailAsync(email);
        if (user == null)
            return NotFound(new { message = $"User with email {email} not found" });
        
        return Ok(user);
    }
    
    [HttpGet("organization/{organization}")]
    [AdminAuthorize]
    public async Task<IActionResult> GetUsersByOrganization(string organization)
    {
        var users = await _userService.GetUsersByOrganizationAsync(organization);
        return Ok(users);
    }
    
    [HttpGet("active")]
    [AdminAuthorize]
    public async Task<IActionResult> GetActiveUsers()
    {
        var users = await _userService.GetActiveUsersAsync();
        return Ok(users);
    }
    
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateUser([FromBody] UserRequestModel request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        try
        {
            var user = new User
            {
                Name = request.Name.Trim(),
                Surname = request.Surname.Trim(),
                Email = request.Email.Trim().ToLower(),
                Occupation = request.Occupation.Trim(),
                Organization = request.Organization.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            
            var created = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }
    
    [HttpPut("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UserRequestModel request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        try
        {
            var user = new User
            {
                Name = request.Name.Trim(),
                Surname = request.Surname.Trim(),
                Email = request.Email.Trim().ToLower(),
                Occupation = request.Occupation.Trim(),
                Organization = request.Organization.Trim()
            };
            
            var updated = await _userService.UpdateUserAsync(id, user);
            if (updated == null)
                return NotFound(new { message = $"User with ID {id} not found" });
            
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }
    
    [HttpDelete("{id}")]
    [AdminAuthorize]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var deleted = await _userService.DeleteUserAsync(id);
        if (!deleted)
            return NotFound(new { message = $"User with ID {id} not found" });
        
        return NoContent();
    }
}