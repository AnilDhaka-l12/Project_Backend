using ProjectBackend.Model;
using System.Threading.Tasks;

namespace ProjectBackend.Services.IServices
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body);
        Task<bool> SendUpdateNotificationAsync(string toEmail, PushNotification notification);
    }
}
