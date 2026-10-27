using LibraryApp.Data;
using LibraryApp.Feature.Quiz.DTOs;
using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Feature.Quiz.Services
{
    public interface IGamificationService
    {
        Task<QuizSubmitResponse> RecordQuizAttemptAsync(Guid userId, QuizSubmitRequest request);
        Task<UserGamificationStatsDTO> GetUserStatsAsync(Guid userId);
        Task<List<LeaderboardEntryDTO>> GetLeaderboardAsync(int top = 50);
        Task<bool> UpdateUserGenresAsync(Guid userId, List<string> genres);
        Task<List<string>> GetUserGenresAsync(Guid userId);
    }

    public class GamificationService : IGamificationService
    {
        private readonly AppDBContext _context;
        private readonly ILogger<GamificationService> _logger;

        public GamificationService(AppDBContext context, ILogger<GamificationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<QuizSubmitResponse> RecordQuizAttemptAsync(Guid userId, QuizSubmitRequest request)
        {
            var user = await _context.Users
                .Include(u => u.Badges)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            // Calculate points: Easy=10 per correct, Medium=20, Hard=30
            int multiplier = request.Difficulty.ToLowerInvariant() switch
            {
                "easy" => 10,
                "hard" => 30,
                _ => 20
            };
            int pointsEarned = request.CorrectAnswers * multiplier;

            // Extra bonus for perfect score
            if (request.CorrectAnswers > 0 && request.CorrectAnswers == request.TotalQuestions)
            {
                pointsEarned += 25;
            }

            user.TotalPoints += pointsEarned;

            // Save Attempt Record
            var attempt = new UserQuizAttempt
            {
                UserId = userId,
                BookId = request.BookId,
                Category = request.Category,
                Difficulty = request.Difficulty,
                Score = request.CorrectAnswers,
                TotalQuestions = request.TotalQuestions,
                PointsEarned = pointsEarned,
                CompletedAt = DateTime.UtcNow
            };
            await _context.UserQuizAttempts.AddAsync(attempt);

            // Streak Tracking Logic
            var today = DateTime.UtcNow.Date;
            if (request.IsDailyRead && request.BookId.HasValue)
            {
                var existingLog = await _context.DailyReadLogs
                    .FirstOrDefaultAsync(d => d.UserId == userId && d.CompletedDate == today);

                if (existingLog == null)
                {
                    await _context.DailyReadLogs.AddAsync(new DailyReadLog
                    {
                        UserId = userId,
                        BookId = request.BookId.Value,
                        Genre = request.Category,
                        CompletedDate = today
                    });

                    // Update Streak
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
                            // Consecutive day!
                            user.CurrentStreak += 1;
                        }
                        else if (lastDate < today.AddDays(-1))
                        {
                            // Streak broken, reset
                            user.CurrentStreak = 1;
                            user.CurrentStreakStartDate = today;
                        }
                        // If lastDate == today, do nothing (already completed today)
                    }

                    user.LastActiveDate = DateTime.UtcNow;

                    // Update longest streak range
                    if (user.CurrentStreak > user.LongestStreak)
                    {
                        user.LongestStreak = user.CurrentStreak;
                        user.LongestStreakStartDate = user.CurrentStreakStartDate ?? today;
                        user.LongestStreakEndDate = today;
                    }
                }
            }

            // Check & Unlock Badges
            var unlockedBadges = new List<string>();
            var allBadges = await _context.Badges.ToListAsync();
            var userBadgeIds = user.Badges.Select(b => b.BadgeId).ToHashSet();

            foreach (var badge in allBadges)
            {
                if (userBadgeIds.Contains(badge.Id)) continue;

                bool eligible = false;
                if (badge.RequiredPoints > 0 && user.TotalPoints >= badge.RequiredPoints)
                {
                    eligible = true;
                }
                if (badge.RequiredStreak > 0 && user.CurrentStreak >= badge.RequiredStreak)
                {
                    eligible = true;
                }

                if (eligible)
                {
                    await _context.UserBadges.AddAsync(new UserBadge
                    {
                        UserId = userId,
                        BadgeId = badge.Id,
                        EarnedAt = DateTime.UtcNow
                    });
                    unlockedBadges.Add(badge.Name);
                }
            }

            await _context.SaveChangesAsync();

            return new QuizSubmitResponse
            {
                PointsEarned = pointsEarned,
                TotalPoints = user.TotalPoints,
                CurrentStreak = user.CurrentStreak,
                LongestStreak = user.LongestStreak,
                LongestStreakStartDate = user.LongestStreakStartDate,
                LongestStreakEndDate = user.LongestStreakEndDate,
                NewBadgesUnlocked = unlockedBadges
            };
        }

        public async Task<UserGamificationStatsDTO> GetUserStatsAsync(Guid userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Badges)
                    .ThenInclude(ub => ub.Badge)
                .Include(u => u.SelectedGenres)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            var today = DateTime.UtcNow.Date;
            var completedToday = await _context.DailyReadLogs
                .AnyAsync(d => d.UserId == userId && d.CompletedDate == today);

            var allBadges = await _context.Badges.AsNoTracking().ToListAsync();
            var earnedMap = user.Badges.ToDictionary(b => b.BadgeId, b => b.EarnedAt);

            var badgeDTOs = allBadges.Select(b => new BadgeDTO
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                Icon = b.Icon,
                IsUnlocked = earnedMap.ContainsKey(b.Id),
                EarnedAt = earnedMap.TryGetValue(b.Id, out var dt) ? dt : null
            }).ToList();

            return new UserGamificationStatsDTO
            {
                TotalPoints = user.TotalPoints,
                CurrentStreak = user.CurrentStreak,
                LongestStreak = user.LongestStreak,
                CurrentStreakStartDate = user.CurrentStreakStartDate,
                LongestStreakStartDate = user.LongestStreakStartDate,
                LongestStreakEndDate = user.LongestStreakEndDate,
                CompletedDailyReadToday = completedToday,
                SelectedGenres = user.SelectedGenres.Select(g => g.Genre).ToList(),
                Badges = badgeDTOs
            };
        }

        public async Task<List<LeaderboardEntryDTO>> GetLeaderboardAsync(int top = 50)
        {
            return await _context.Users
                .AsNoTracking()
                .OrderByDescending(u => u.TotalPoints)
                .ThenByDescending(u => u.CurrentStreak)
                .Take(top)
                .Select(u => new LeaderboardEntryDTO
                {
                    UserId = u.Id,
                    Name = u.Name,
                    TotalPoints = u.TotalPoints,
                    CurrentStreak = u.CurrentStreak,
                    LongestStreak = u.LongestStreak,
                    BadgeCount = u.Badges.Count
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateUserGenresAsync(Guid userId, List<string> genres)
        {
            var existing = await _context.UserGenres.Where(g => g.UserId == userId).ToListAsync();
            _context.UserGenres.RemoveRange(existing);

            var distinctGenres = genres.Distinct().Where(g => !string.IsNullOrWhiteSpace(g)).ToList();
            foreach (var genre in distinctGenres)
            {
                await _context.UserGenres.AddAsync(new UserGenre
                {
                    UserId = userId,
                    Genre = genre.Trim(),
                    SelectedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<string>> GetUserGenresAsync(Guid userId)
        {
            return await _context.UserGenres
                .AsNoTracking()
                .Where(g => g.UserId == userId)
                .Select(g => g.Genre)
                .ToListAsync();
        }
    }
}
