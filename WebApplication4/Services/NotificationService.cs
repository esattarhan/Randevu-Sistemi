using WebApplication4.Models;

namespace WebApplication4.Services
{
    public static class NotificationService
    {
        public static void NotifyByEmail(string to, string subject, string body)
        {
            // Stub: Gerçek e-posta entegrasyonu eklenebilir (SMTP veya 3rd party)
            System.Diagnostics.Debug.WriteLine($"EMAIL to={to} subject={subject} body={body}");
        }

        public static void NotifyBySms(string phone, string message)
        {
            // Stub: Gerçek SMS entegrasyonu eklenebilir
            System.Diagnostics.Debug.WriteLine($"SMS to={phone} msg={message}");
        }
    }
}
