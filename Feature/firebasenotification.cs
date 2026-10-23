using FirebaseAdmin.Messaging;
using Google;
using LibraryApp.Data;
using Microsoft.EntityFrameworkCore;
public interface INotificationService
{
    Task NotifyAdminsAsync(string title, string body);
    Task BroadcastAsync(string title, string body);
    Task NotifyUserAsync(Guid userId, string title, string body);
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

        await SendMulticastAsync(adminTokens, title, body);
    }

    public async Task BroadcastAsync(string title, string body)
    {
        var allTokens = await _context.UserFcmTokens
            .Where(t => !string.IsNullOrEmpty(t.FcmToken))
            .Select(t => t.FcmToken)
            .Distinct()
            .ToListAsync();

        await SendMulticastAsync(allTokens, title, body);
    }

    public async Task NotifyUserAsync(Guid userId, string title, string body)
    {
        var userTokens = await _context.UserFcmTokens
            .Where(t => t.UserId == userId && !string.IsNullOrEmpty(t.FcmToken))
            .Select(t => t.FcmToken)
            .Distinct()
            .ToListAsync();

        await SendMulticastAsync(userTokens, title, body);
    }

    private async Task SendMulticastAsync(List<string> tokens, string title, string body)
    {
        if (tokens == null || tokens.Count == 0) return;

        // Firebase multicast sends in batches of up to 500
        const int batchSize = 500;
        for (int i = 0; i < tokens.Count; i += batchSize)
        {
            var batch = tokens.Skip(i).Take(batchSize).ToList();
            var multicast = new MulticastMessage
            {
                Tokens = batch,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                }
            };

            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(multicast);
                Console.WriteLine($"[FCM] Sent to {response.SuccessCount}/{batch.Count} devices. Failures: {response.FailureCount}");

                if (response.FailureCount > 0)
                {
                    var deadTokens = new List<string>();
                    for (int j = 0; j < response.Responses.Count; j++)
                    {
                        var item = response.Responses[j];
                        if (!item.IsSuccess)
                        {
                            var token = batch[j];
                            Console.WriteLine($"[FCM Token Failure]: {item.Exception?.MessagingErrorCode} - {item.Exception?.Message} (Token: {token.Substring(0, Math.Min(token.Length, 20))}...)");

                            // If token is invalid, unregistered, or bogus mock token, mark for removal
                            if (item.Exception?.MessagingErrorCode == MessagingErrorCode.Unregistered ||
                                item.Exception?.MessagingErrorCode == MessagingErrorCode.InvalidArgument)
                            {
                                deadTokens.Add(token);
                            }
                        }
                    }

                    if (deadTokens.Count > 0)
                    {
                        var staleEntities = await _context.UserFcmTokens
                            .Where(t => deadTokens.Contains(t.FcmToken))
                            .ToListAsync();
                        if (staleEntities.Count > 0)
                        {
                            _context.UserFcmTokens.RemoveRange(staleEntities);
                            await _context.SaveChangesAsync();
                            Console.WriteLine($"[FCM Cleanup]: Removed {staleEntities.Count} stale/invalid token(s) from database.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FCM ERROR] Failed to send multicast batch: {ex.Message}");
            }
        }
    }
}