using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class DailyReadLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public int BookId { get; set; }

        [MaxLength(100)]
        public string Genre { get; set; } = string.Empty;

        public DateTime CompletedDate { get; set; } = DateTime.UtcNow.Date;
    }
}
