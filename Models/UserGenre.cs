using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class UserGenre
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }
        public User? User { get; set; }

        [Required]
        [MaxLength(100)]
        public string Genre { get; set; } = string.Empty;

        public DateTime SelectedAt { get; set; } = DateTime.UtcNow;
    }
}
