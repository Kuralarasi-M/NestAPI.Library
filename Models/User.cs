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
        public List<Favorite> Favorites { get; set; } = new();
        public List<ReadingHistory> ReadingHistories { get; set; } = new();
        public List<DownloadHistory> DownloadHistories { get; set; } = new ();
        public List<SearchHistory> SearchHistories { get; set; } = new();
        public List<UserFcmToken> FcmTokens { get; set; } = new();
        public List<Notification> Notifications { get; set; } = new();

    }
}
