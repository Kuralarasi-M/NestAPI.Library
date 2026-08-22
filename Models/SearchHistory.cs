using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models;

public class SearchHistory
{
    public int Id { get; set; }

    public int UserId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Query { get; set; } = string.Empty;

    public DateTime SearchAt { get; set; } = DateTime.UtcNow;
    public Book Book { get; set; } = null!;
    public User User { get; set; } = null!;
}