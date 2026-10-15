using FirebaseAdmin.Messaging;
using Google;
using LibraryApp.Data;
using Microsoft.EntityFrameworkCore;
public interface INotificationService
{
    Task NotifyAdminsAsync(string title, string body);
}
public class NotificationService : INotificationService
{
    private readonly AppDBContext _context;

    public NotificationService(AppDBContext context)
    {
        _context = context;
    }

    public async Task NotifyAdminsAsync(string title, string body)
    {
        var adminTokens = await (
     from user in _context.Users
     join token in _context.UserFcmTokens
         on user.Id equals token.UserId
     where user.Role == "Admin"
           && !string.IsNullOrEmpty(token.FcmToken)
     select token.FcmToken
 )
 .Distinct()
 .ToListAsync();

        foreach (var token in adminTokens)
        {
            var message = new Message
            {
                Token = token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                }
            };
            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);

                Console.WriteLine("=================================");
                Console.WriteLine("FCM SENT SUCCESSFULLY");
                Console.WriteLine($"Response: {response}");
                Console.WriteLine("=================================");
            }
            catch (FirebaseMessagingException ex)
            {
                Console.WriteLine("=================================");
                Console.WriteLine("FCM FAILED");
                Console.WriteLine($"ErrorCode: {ex.MessagingErrorCode}");
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("=================================");

                throw;
            }
        }
    }
}