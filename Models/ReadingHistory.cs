namespace LibraryApp.Models;

public class ReadingHistory
{
    public long Id { get; set; }

    public Guid UserId { get; set; }

    public long BookId { get; set; }

    public DateTime LastReadAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;

    public Book Book { get; set; } = null!;
}