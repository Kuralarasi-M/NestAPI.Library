using System.ComponentModel.DataAnnotations;
namespace LibraryApp.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MaxLength(20)]
        public string PasswordHash { get; set; }
        public string Role { get; set; } = "User";
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Gamification & Streak tracking
        public int TotalPoints { get; set; } = 0;
        public int WordSearchStage { get; set; } = 1;
        public int CurrentStreak { get; set; } = 0;
        public int LongestStreak { get; set; } = 0;
        public DateTime? LongestStreakStartDate { get; set; }
        public DateTime? LongestStreakEndDate { get; set; }
        public DateTime? CurrentStreakStartDate { get; set; }
        public DateTime? LastActiveDate { get; set; }

        public List<Favorite> Favorites { get; set; } = new();
        public List<ReadingHistory> ReadingHistories { get; set; } = new();
        public List<DownloadHistory> DownloadHistories { get; set; } = new ();
        public List<SearchHistory> SearchHistories { get; set; } = new();
        public List<UserFcmToken> FcmTokens { get; set; } = new();
        public List<Notification> Notifications { get; set; } = new();
        public List<UserGenre> SelectedGenres { get; set; } = new();
        public List<UserQuizAttempt> QuizAttempts { get; set; } = new();
        public List<UserBadge> Badges { get; set; } = new();
        public List<BookReview> Reviews { get; set; } = new();
        public List<DailyReadLog> DailyReadLogs { get; set; } = new();
    }
}
