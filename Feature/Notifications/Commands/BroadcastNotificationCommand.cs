using LibraryApp.Data;
using LibraryApp.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Feature.Notifications.Commands
{
    public class BroadcastNotificationCommand : IRequest<BroadcastResponse>
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string TargetAudience { get; set; } = "all"; // "all" or "admins"
        public Guid SentByAdminId { get; set; }
    }

    public class BroadcastResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int RecipientsCount { get; set; }
    }

    public class BroadcastNotificationHandler : IRequestHandler<BroadcastNotificationCommand, BroadcastResponse>
    {
        private readonly AppDBContext _context;
        private readonly INotificationService _notificationService;

        public BroadcastNotificationHandler(AppDBContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<BroadcastResponse> Handle(BroadcastNotificationCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Message))
            {
                return new BroadcastResponse
                {
                    Success = false,
                    Message = "Title and Message are required."
                };
            }

            // 1. Get recipients
            var usersQuery = _context.Users.AsQueryable();
            if (request.TargetAudience.ToLower() == "admins")
            {
                usersQuery = usersQuery.Where(u => u.Role == "Admin");
            }

            var recipients = await usersQuery.Select(u => u.Id).ToListAsync(cancellationToken);
            if (recipients.Count == 0)
            {
                return new BroadcastResponse
                {
                    Success = false,
                    Message = "No recipients found for the target audience."
                };
            }

            // 2. Save in-app notification in DB for each recipient
            var notifications = recipients.Select(userId => new Notification
            {
                UserId = userId,
                Title = request.Title.Trim(),
                Message = request.Message.Trim(),
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _context.Notifications.AddRangeAsync(notifications, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // 3. Send FCM Push Notification
            try
            {
                if (request.TargetAudience.ToLower() == "admins")
                {
                    await _notificationService.NotifyAdminsAsync(request.Title.Trim(), request.Message.Trim());
                }
                else
                {
                    await _notificationService.BroadcastAsync(request.Title.Trim(), request.Message.Trim());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FCM Broadcast Error]: {ex.Message}");
            }

            return new BroadcastResponse
            {
                Success = true,
                Message = $"Broadcast notification sent to {recipients.Count} users successfully.",
                RecipientsCount = recipients.Count
            };
        }
    }
}
