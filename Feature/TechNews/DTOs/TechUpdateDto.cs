namespace LibraryApp.Feature.TechNews.DTOs;

public class TechUpdateDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string SourceUrl { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public string Category { get; set; } = "News"; // Release, News, Tutorial, Updates, Announcement
    public List<string> Tags { get; set; } = new();
    public int Upvotes { get; set; }
    public int CommentsCount { get; set; }
    public string? Author { get; set; }
    public string? Domain { get; set; }
}

public class TechNewsQueryParameters
{
    public string? Category { get; set; }
    public string? Technology { get; set; }
    public string? Search { get; set; }
    public string? Source { get; set; }
    public string? Sort { get; set; } = "latest"; // latest, popular
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class TechNewsPagedResponse
{
    public List<TechUpdateDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / Math.Max(1, PageSize));
    public DateTime LastUpdatedAt { get; set; }
}
