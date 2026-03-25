using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace projectBackend.Config.MailKit
{
    public class MailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MailService> _logger;
        private SmtpClient _smtpClient;
        private bool _isInitialized = false;
        
        public MailService(IConfiguration configuration, ILogger<MailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        
        // Auto-initialize when first used
        private async Task EnsureInitializedAsync()
        {
            if (!_isInitialized)
            {
                try
                {
                    _smtpClient = new SmtpClient();
                    var host = _configuration["EmailSettings:Host"] ?? "smtp.gmail.com";
                    var port = int.TryParse(_configuration["EmailSettings:Port"], out int p) ? p : 587;
                    var login = _configuration["EmailSettings:Login"] ?? string.Empty;
                    var key = _configuration["EmailSettings:Key"] ?? string.Empty;
                    
                    await _smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                    await _smtpClient.AuthenticateAsync(login, key);
                    
                    _isInitialized = true;
                    _logger.LogInformation("Mail service initialized successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initialize mail service");
                    throw;
                }
            }
        }
        
        public async Task SendRawEmailAsync(MimeMessage message)
        {
            await EnsureInitializedAsync(); // Auto-initialize on first use
            
            if (_smtpClient == null || !_smtpClient.IsConnected)
            {
                throw new InvalidOperationException("Mail service not connected");
            }
            
            await _smtpClient.SendAsync(message);
        }
        
        public async Task DisconnectAsync()
        {
            if (_smtpClient != null && _smtpClient.IsConnected)
            {
                await _smtpClient.DisconnectAsync(true);
                _smtpClient.Dispose();
                _isInitialized = false;
            }
        }
    }
}