using System.Text.Json.Serialization;

namespace LibraryApp.Feature.Quiz.DTOs
{
    public class OpenTdbResponse
    {
        [JsonPropertyName("response_code")]
        public int ResponseCode { get; set; }

        [JsonPropertyName("results")]
        public List<OpenTdbQuestionItem> Results { get; set; } = new();
    }

    public class OpenTdbQuestionItem
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("difficulty")]
        public string Difficulty { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("question")]
        public string Question { get; set; } = string.Empty;

        [JsonPropertyName("correct_answer")]
        public string CorrectAnswer { get; set; } = string.Empty;

        [JsonPropertyName("incorrect_answers")]
        public List<string> IncorrectAnswers { get; set; } = new();
    }

    public class QuizQuestionDTO
    {
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
        public int CorrectOptionIndex { get; set; }
    }

    public class QuizSubmitRequest
    {
        public int? BookId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Difficulty { get; set; } = "medium";
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public bool IsDailyRead { get; set; } = false;
    }

    public class QuizSubmitResponse
    {
        public int PointsEarned { get; set; }
        public int TotalPoints { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public DateTime? LongestStreakStartDate { get; set; }
        public DateTime? LongestStreakEndDate { get; set; }
        public List<string> NewBadgesUnlocked { get; set; } = new();
    }

    public class UserGamificationStatsDTO
    {
        public int TotalPoints { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public DateTime? CurrentStreakStartDate { get; set; }
        public DateTime? LongestStreakStartDate { get; set; }
        public DateTime? LongestStreakEndDate { get; set; }
        public bool CompletedDailyReadToday { get; set; }
        public List<string> SelectedGenres { get; set; } = new();
        public List<BadgeDTO> Badges { get; set; } = new();
    }

    public class BadgeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public bool IsUnlocked { get; set; }
        public DateTime? EarnedAt { get; set; }
    }

    public class LeaderboardEntryDTO
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public int BadgeCount { get; set; }
    }

    public class BookReviewDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int BookId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateReviewRequest
    {
        public int BookId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    public class SetGenresRequest
    {
        public List<string> Genres { get; set; } = new();
    }
}
