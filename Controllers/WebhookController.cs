using Microsoft.AspNetCore.Mvc;
using projectBackend.Model;
using projectBackend.Services.IServices;

namespace EmailBackend.Controllers;

[ApiController]
[Route("api")]
public class WebhookController : ControllerBase
{
    private readonly ILogger<WebhookController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public WebhookController(
        ILogger<WebhookController> logger, 
        IConfiguration configuration,
        IEmailService emailService)
    {
        _logger = logger;
        _configuration = configuration;
        _emailService = emailService;
    }

    [HttpPost("notify-push")]
    public async Task<IActionResult> NotifyPush([FromBody] PushNotification notification)
    {
        try
        {
            _logger.LogInformation("=== Webhook Received ===");
            _logger.LogInformation($"Commit: {notification.Commit}");
            _logger.LogInformation($"Message: {notification.Message}");
            _logger.LogInformation($"Author: {notification.Author}");
            _logger.LogInformation($"Timestamp: {DateTime.Now}");

            // Get users to notify from configuration
            var usersToNotify = _configuration["Users:Emails"]?
                .Split(',', StringSplitOptions.RemoveEmptyEntries) 
                ?? new[] { "your-email@example.com" };

            // Send email to each user
            foreach (var userEmail in usersToNotify)
            {
                await _emailService.SendUpdateNotificationAsync(userEmail.Trim(), notification);
                _logger.LogInformation($"Notification sent to {userEmail}");
            }

            return Ok(new { 
                success = true, 
                message = $"Notification sent to {usersToNotify.Length} user(s) for commit {notification.Commit?[..Math.Min(7, notification.Commit?.Length ?? 7)]}" 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook");
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }
}