using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class BookReview
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }
        public User? User { get; set; }

        [Required]
        public int BookId { get; set; } // Gutendex ID

        [Range(1, 5)]
        public int Rating { get; set; } = 5;

        [Required]
        [MaxLength(2000)]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
