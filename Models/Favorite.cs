namespace LibraryApp.Models;

public class Favorite
{
    public long Id { get; set; }

    public int UserId { get; set; }

    public long BookId { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;

    public Book Book { get; set; } = null!;
}