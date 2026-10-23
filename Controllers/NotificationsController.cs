using LibraryApp.Feature.Auth.Commands;
using LibraryApp.Feature.Notifications.Commands;
using LibraryApp.Data;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LibraryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly AppDBContext _context;

        public NotificationsController(IMediator mediator, AppDBContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        private Guid GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value
                       ?? User.FindFirst("id")?.Value;

            if (Guid.TryParse(idClaim, out var guid))
                return guid;

            return Guid.Empty;
        }

        private bool IsCurrentUserAdmin()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value
                         ?? User.FindFirst("role")?.Value;

            return string.Equals(roleClaim, "Admin", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Get in-app notifications for the logged-in user (both User and Admin).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var list = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(50)
                .Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Message,
                    n.IsRead,
                    n.CreatedAt
                })
                .ToListAsync();

            return Ok(list);
        }

        /// <summary>
        /// Get unread notification count for the badge.
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var count = await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);

            return Ok(new { unreadCount = count });
        }

        /// <summary>
        /// Mark a single notification as read.
        /// </summary>
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = GetCurrentUserId();
            var item = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
            if (item == null)
                return NotFound();

            item.IsRead = true;
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }

        /// <summary>
        /// Mark all notifications as read for current user.
        /// </summary>
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            var unread = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var n in unread)
            {
                n.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return Ok(new { success = true, markedCount = unread.Count });
        }

        /// <summary>
        /// Broadcast notification to all users or admins. Restricted to Admin role only.
        /// Saves notification in DB for each user and sends FCM push notification.
        /// </summary>
        [HttpPost("broadcast")]
        public async Task<IActionResult> Broadcast([FromBody] BroadcastNotificationCommand command)
        {
            if (!IsCurrentUserAdmin())
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins can send broadcasts." });
            }

            command.SentByAdminId = GetCurrentUserId();
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
