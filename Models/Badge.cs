using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class Badge
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Icon { get; set; } = "Award";

        public int RequiredPoints { get; set; } = 0;
        public int RequiredStreak { get; set; } = 0;
    }

    public class UserBadge
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }
        public User? User { get; set; }

        [Required]
        public int BadgeId { get; set; }
        public Badge? Badge { get; set; }

        public DateTime EarnedAt { get; set; } = DateTime.UtcNow;
    }
}
