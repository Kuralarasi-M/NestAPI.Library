using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models;

public class Book
{
    public long Id { get; set; }

    public int GutendexId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string Author { get; set; } = string.Empty;

    public string CoverImage { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Language { get; set; } = string.Empty;

    // Stores multiple genres/categories separated by commas
    public string Genre { get; set; } = string.Empty;

    // Book description from Gutendex
    public string Summary { get; set; } = string.Empty;

    // Stores multiple subjects separated by commas
    public string Subjects { get; set; } = string.Empty;

    // Number of downloads from Gutendex
    public int DownloadCount { get; set; }

    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public ICollection<ReadingHistory> ReadingHistories { get; set; } = new List<ReadingHistory>();

    public ICollection<DownloadHistory> DownloadHistories { get; set; } = new List<DownloadHistory>();

    public ICollection<SearchHistory> SearchHistories { get; set; } = new List<SearchHistory>();
}