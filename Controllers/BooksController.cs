using LibraryApp.Data;
using LibraryApp.Feature.Auth.Commands;
using LibraryApp.Feature.Auth.Handler;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace LibraryApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly AppDBContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;

        public BooksController(
            IMediator mediator,
            AppDBContext context,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache)
        {
            _mediator = mediator;
            _context = context;
            _httpClientFactory = httpClientFactory;
            _cache = cache;
        }

        [HttpGet("Language/{languageCode}")]
        public async Task<IActionResult> GetBooksByLanguage(
            string languageCode,
            [FromQuery] string? query = null,
            [FromQuery] string? topic = null,
            CancellationToken cancellationToken = default)
        {
            var lang = string.IsNullOrWhiteSpace(languageCode) ? "all" : languageCode.Trim().ToLowerInvariant();
            var cacheKey = $"books_lang_{lang}_{query?.Trim().ToLowerInvariant()}_{topic?.Trim().ToLowerInvariant()}";

            if (_cache.TryGetValue(cacheKey, out List<BooksResponse>? cachedBooks) && cachedBooks != null)
            {
                return Ok(cachedBooks);
            }

            var results = new List<BooksResponse>();

            // 1. Fetch from Gutendex API via HttpClient
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://gutendex.com/");
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

                var queryParams = new List<string>();
                if (lang != "all")
                {
                    queryParams.Add($"languages={Uri.EscapeDataString(lang)}");
                }
                if (!string.IsNullOrWhiteSpace(query))
                {
                    queryParams.Add($"search={Uri.EscapeDataString(query.Trim())}");
                }
                if (!string.IsNullOrWhiteSpace(topic) && topic != "All")
                {
                    queryParams.Add($"topic={Uri.EscapeDataString(topic.Trim())}");
                }

                var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
                var response = await client.GetAsync($"books/{queryString}", cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync(cancellationToken);
                    using var doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("results", out var resultsElement) && resultsElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in resultsElement.EnumerateArray())
                        {
                            var id = item.GetProperty("id").GetInt32();
                            var title = item.TryGetProperty("title", out var titleProp) ? titleProp.GetString() ?? "Untitled" : "Untitled";

                            var authorName = "Unknown Author";
                            if (item.TryGetProperty("authors", out var authorsProp) && authorsProp.ValueKind == JsonValueKind.Array)
                            {
                                var names = authorsProp.EnumerateArray()
                                    .Select(a => a.TryGetProperty("name", out var n) ? n.GetString() : null)
                                    .Where(n => !string.IsNullOrEmpty(n))
                                    .ToList();
                                if (names.Count > 0) authorName = string.Join(", ", names);
                            }

                            var coverImage = $"https://www.gutenberg.org/cache/epub/{id}/pg{id}.cover.medium.jpg";
                            if (item.TryGetProperty("formats", out var formatsProp) && formatsProp.ValueKind == JsonValueKind.Object)
                            {
                                if (formatsProp.TryGetProperty("image/jpeg", out var img))
                                {
                                    coverImage = img.GetString() ?? coverImage;
                                }
                            }

                            var itemLang = lang;
                            if (item.TryGetProperty("languages", out var langsProp) && langsProp.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var l in langsProp.EnumerateArray())
                                {
                                    itemLang = l.GetString() ?? itemLang;
                                    break;
                                }
                            }

                            var subjects = new List<string>();
                            if (item.TryGetProperty("subjects", out var subProp) && subProp.ValueKind == JsonValueKind.Array)
                            {
                                subjects = subProp.EnumerateArray()
                                    .Select(s => s.GetString() ?? "")
                                    .Where(s => !string.IsNullOrEmpty(s))
                                    .ToList();
                            }

                            var summary = $"Classic work by {authorName}.";
                            if (item.TryGetProperty("summaries", out var sumProp) && sumProp.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var s in sumProp.EnumerateArray())
                                {
                                    summary = s.GetString() ?? summary;
                                    break;
                                }
                            }

                            var downloadCount = item.TryGetProperty("download_count", out var dc) ? dc.GetInt32() : 0;

                            results.Add(new BooksResponse
                            {
                                Id = id,
                                GutendexId = id,
                                Title = title,
                                Author = authorName,
                                CoverImage = coverImage,
                                Language = itemLang,
                                Genre = subjects.Count > 0 ? subjects[0] : "General",
                                Subjects = string.Join("; ", subjects),
                                Summary = summary,
                                DownloadCount = downloadCount
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gutendex language fetch error: {ex.Message}");
            }

            // 2. Fallback to local DB if Gutendex returned nothing or for Tamil supplementary books
            if (results.Count == 0 || lang == "ta")
            {
                var dbQuery = _context.Books.AsQueryable();

                if (lang == "ta")
                {
                    dbQuery = dbQuery.Where(b =>
                        (b.Language != null && EF.Functions.ILike(b.Language, "%ta%")) ||
                        (b.Genre != null && EF.Functions.ILike(b.Genre, "%tamil%")) ||
                        (b.Title != null && EF.Functions.ILike(b.Title, "%tamil%")));
                }
                else if (lang != "all")
                {
                    dbQuery = dbQuery.Where(b => b.Language != null && EF.Functions.ILike(b.Language, lang));
                }

                if (!string.IsNullOrWhiteSpace(query))
                {
                    var pattern = $"%{query.Trim()}%";
                    dbQuery = dbQuery.Where(b =>
                        (b.Title != null && EF.Functions.ILike(b.Title, pattern)) ||
                        (b.Author != null && EF.Functions.ILike(b.Author, pattern)));
                }

                var dbBooks = await dbQuery
                    .OrderByDescending(b => b.DownloadCount)
                    .Select(book => new BooksResponse
                    {
                        Id = book.Id,
                        GutendexId = book.GutendexId,
                        CoverImage = book.CoverImage,
                        Language = book.Language,
                        Genre = book.Genre,
                        DownloadCount = book.DownloadCount,
                        Subjects = book.Subjects,
                        Summary = book.Summary,
                        Title = book.Title,
                        Author = book.Author
                    })
                    .ToListAsync(cancellationToken);

                // Merge and deduplicate by GutendexId
                foreach (var dbBook in dbBooks)
                {
                    if (!results.Any(r => r.GutendexId == dbBook.GutendexId))
                    {
                        results.Add(dbBook);
                    }
                }
            }

            // Cache for 30 minutes to make subsequent language switching instantaneous
            if (results.Count > 0)
            {
                _cache.Set(cacheKey, results, TimeSpan.FromMinutes(30));
            }

            return Ok(results);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] string? query, [FromQuery] string? language)
        {
            var booksQuery = _context.Books.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var pattern = $"%{query.Trim()}%";
                booksQuery = booksQuery.Where(b =>
                    (b.Title != null && EF.Functions.ILike(b.Title, pattern)) ||
                    (b.Author != null && EF.Functions.ILike(b.Author, pattern)) ||
                    (b.Genre != null && EF.Functions.ILike(b.Genre, pattern)) ||
                    (b.Subjects != null && EF.Functions.ILike(b.Subjects, pattern)) ||
                    (b.Summary != null && EF.Functions.ILike(b.Summary, pattern)));
            }

            if (!string.IsNullOrWhiteSpace(language) && language != "all")
            {
                var lang = language.Trim();
                if (string.Equals(lang, "ta", StringComparison.OrdinalIgnoreCase))
                {
                    booksQuery = booksQuery.Where(b =>
                        (b.Language != null && EF.Functions.ILike(b.Language, "%ta%")) ||
                        (b.Genre != null && EF.Functions.ILike(b.Genre, "%tamil%")) ||
                        (b.Title != null && EF.Functions.ILike(b.Title, "%tamil%")));
                }
                else
                {
                    booksQuery = booksQuery.Where(b => b.Language != null && EF.Functions.ILike(b.Language, lang));
                }
            }

            var results = await booksQuery
                .OrderByDescending(b => b.DownloadCount)
                .Select(book => new BooksResponse
                {
                    Id = book.Id,
                    GutendexId = book.GutendexId,
                    CoverImage = book.CoverImage,
                    Language = book.Language,
                    Genre = book.Genre,
                    DownloadCount = book.DownloadCount,
                    Subjects = book.Subjects,
                    Summary = book.Summary,
                    Title = book.Title,
                    Author = book.Author
                })
                .ToListAsync();

            return Ok(results);
        }

        [HttpGet("{gutendexId:int}")]
        public async Task<IActionResult> GetBookById(int gutendexId)
        {
            var book = await _mediator.Send(new GetBookQuery(gutendexId));

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpGet("{gutendexId:int}/content")]
        public async Task<IActionResult> GetBookContent(int gutendexId)
        {
            var content = await _mediator.Send(new GetBookContentQuery(gutendexId));
            return Ok(content);
        }

        [HttpGet("AllBooks")]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _mediator.Send(new AllBooksQuery());
            if(books == null)
            {
                return NotFound();
            }
            return Ok(books);
        }
    }
}
