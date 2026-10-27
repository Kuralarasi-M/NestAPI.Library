using LibraryApp.Data;
using LibraryApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyTamilController : ControllerBase
    {
        private readonly AppDBContext _context;

        public DailyTamilController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet("Today")]
        public async Task<IActionResult> GetTodaySentences()
        {
            // Deterministic calculation: changes every 24 hours based on DayOfYear
            int totalAvailable = await _context.DailyTamilSentences.CountAsync();
            if (totalAvailable == 0)
            {
                return Ok(new List<DailyTamilSentence>());
            }

            int dayOfYear = DateTime.UtcNow.DayOfYear;
            int batches = Math.Max(1, totalAvailable / 5);
            int selectedDayNumber = (dayOfYear % batches) + 1;

            var sentences = await _context.DailyTamilSentences
                .AsNoTracking()
                .Where(s => s.DayNumber == selectedDayNumber)
                .Take(5)
                .ToListAsync();

            // If selectedDayNumber has fewer than 5, grab the first 5 available
            if (sentences.Count < 5)
            {
                sentences = await _context.DailyTamilSentences
                    .AsNoTracking()
                    .Take(5)
                    .ToListAsync();
            }

            return Ok(sentences);
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllSentences([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var items = await _context.DailyTamilSentences
                .AsNoTracking()
                .OrderBy(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(items);
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("nameid")?.Value
                ?? User.FindFirst("userId")?.Value;

            if (Guid.TryParse(userIdClaim, out var id))
            {
                return id;
            }
            return null;
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost("CompleteDailyReading")]
        public async Task<IActionResult> CompleteDailyReading()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized("User not authenticated");

            var user = await _context.Users
                .Include(u => u.Badges)
                .FirstOrDefaultAsync(u => u.Id == userId.Value);

            if (user == null) return NotFound("User not found");

            var today = DateTime.UtcNow.Date;
            var existingLog = await _context.DailyReadLogs
                .FirstOrDefaultAsync(d => d.UserId == userId.Value && d.CompletedDate == today && d.Genre == "DailyTamil");

            bool isNewToday = false;
            int pointsEarned = 0;

            if (existingLog == null)
            {
                isNewToday = true;
                pointsEarned = 50; // Award 50 points for completing daily 5 words
                user.TotalPoints += pointsEarned;

                await _context.DailyReadLogs.AddAsync(new DailyReadLog
                {
                    UserId = userId.Value,
                    BookId = 0,
                    Genre = "DailyTamil",
                    CompletedDate = today
                });

                // Streak calculation
                if (!user.LastActiveDate.HasValue)
                {
                    user.CurrentStreak = 1;
                    user.CurrentStreakStartDate = today;
                }
                else
                {
                    var lastDate = user.LastActiveDate.Value.Date;
                    if (lastDate == today.AddDays(-1))
                    {
                        // Consecutive day streak increment
                        user.CurrentStreak += 1;
                    }
                    else if (lastDate < today.AddDays(-1))
                    {
                        // Reset streak if missed
                        user.CurrentStreak = 1;
                        user.CurrentStreakStartDate = today;
                    }
                }

                user.LastActiveDate = DateTime.UtcNow;

                if (user.CurrentStreak > user.LongestStreak)
                {
                    user.LongestStreak = user.CurrentStreak;
                    user.LongestStreakStartDate = user.CurrentStreakStartDate ?? today;
                    user.LongestStreakEndDate = today;
                }

                // Check badges
                var allBadges = await _context.Badges.ToListAsync();
                var userBadgeIds = user.Badges.Select(b => b.BadgeId).ToHashSet();
                foreach (var badge in allBadges)
                {
                    if (userBadgeIds.Contains(badge.Id)) continue;
                    bool eligible = false;
                    if (badge.RequiredPoints > 0 && user.TotalPoints >= badge.RequiredPoints) eligible = true;
                    if (badge.RequiredStreak > 0 && user.CurrentStreak >= badge.RequiredStreak) eligible = true;
                    if (eligible)
                    {
                        await _context.UserBadges.AddAsync(new UserBadge
                        {
                            UserId = user.Id,
                            BadgeId = badge.Id,
                            EarnedAt = DateTime.UtcNow
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                success = true,
                isNewToday,
                currentStreak = user.CurrentStreak,
                longestStreak = user.LongestStreak,
                totalPoints = user.TotalPoints,
                pointsEarned,
                message = isNewToday
                    ? $"Awesome! You completed today's 5 words. Streak is now {user.CurrentStreak} days! (+{pointsEarned} points)"
                    : $"You already completed today's words. Streak is {user.CurrentStreak} days."
            });
        }
    }
}
