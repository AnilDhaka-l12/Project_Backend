using MimeKit;
using projectBackend.Config.MailKit;
using projectBackend.Model;
using projectBackend.Services.IServices;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace projectBackend.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailService _mailService;
        private readonly IConfiguration _configuration;
        
        public EmailService(MailService mailService, IConfiguration configuration)
        {
            _mailService = mailService;
            _configuration = configuration;
        }
        
        public async Task<bool> SendUpdateNotificationAsync(string toEmail, PushNotification notification)
        {
            try
            {
                var message = new MimeMessage();
                var fromEmail = _configuration["EmailSettings:FromEmail"] ?? _configuration["EmailSettings:Login"];
                var fromName = _configuration["EmailSettings:FromName"] ?? "App Updates";
                
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                
                message.Subject = $"📦 New Update: {notification.Message?[..Math.Min(50, notification.Message?.Length ?? 50)]}";
                
                message.Body = new TextPart("html") 
                { 
                    Text = $@"
                    <div style='font-family: Arial, sans-serif;'>
                        <h2>New Update Available!</h2>
                        <p><strong>Commit:</strong> {notification.Commit?[..Math.Min(7, notification.Commit?.Length ?? 7)]}</p>
                        <p><strong>Author:</strong> {notification.Author}</p>
                        <p><strong>Message:</strong> {notification.Message}</p>
                    </div>"
                };
                
                await _mailService.SendRawEmailAsync(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                var fromEmail = _configuration["EmailSettings:FromEmail"] ?? _configuration["EmailSettings:Login"];
                var fromName = _configuration["EmailSettings:FromName"] ?? "App Updates";
                
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = body };
                
                await _mailService.SendRawEmailAsync(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}