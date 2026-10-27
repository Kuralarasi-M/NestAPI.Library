using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class UserQuizAttempt
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public int? BookId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Difficulty { get; set; } = "medium"; // easy, medium, hard

        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public int PointsEarned { get; set; }

        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    }
}
