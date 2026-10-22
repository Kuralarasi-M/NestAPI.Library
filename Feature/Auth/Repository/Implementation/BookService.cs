using LibraryApp.Data;
using LibraryApp.Feature.Auth.Commands;
using LibraryApp.Feature.Auth.Repository.Interface;
using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace LibraryApp.Feature.Auth.Repository.Implementation
{
    public class BookService : IBookService
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClient _gutenbergClient;
        private readonly AppDBContext _dbContext;
        private readonly IMemoryCache _cache;

        public BookService(
            HttpClient httpClient,
            AppDBContext context,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache)
        {
            _httpClient = httpClient;
            _dbContext = context;
            _gutenbergClient = httpClientFactory.CreateClient("Gutenberg");
            _cache = cache;
        }

        public async Task<GetBookResponseDTO?> GetBook(
            GetBookQuery gutendexId,
            CancellationToken cancellationToken)
        {
            var cacheKey = $"book_meta_{gutendexId.BookId}";
            if (_cache.TryGetValue(cacheKey, out GetBookResponseDTO? cachedBook) && cachedBook != null)
            {
                return cachedBook;
            }

            GetBookResponseDTO? book = null;
            try
            {
                // Fetch fast metadata from Gutendex (ensure trailing slash)
                var response = await _httpClient.GetAsync(
                    $"books/{gutendexId.BookId}/",
                    cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync(cancellationToken);
                    book = JsonSerializer.Deserialize<GetBookResponseDTO>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gutendex fetch failed for {gutendexId.BookId}: {ex.Message}");
            }

            if (book == null)
            {
                // Fallback to local DB if available
                var dbBook = await _dbContext.Books.FirstOrDefaultAsync(b => b.GutendexId == gutendexId.BookId, cancellationToken);
                if (dbBook != null)
                {
                    book = new GetBookResponseDTO
                    {
                        Id = dbBook.GutendexId,
                        Title = dbBook.Title,
                        Authors = [new AuthorDTO { Name = dbBook.Author }],
                        Summaries = string.IsNullOrEmpty(dbBook.Summary) ? [] : [dbBook.Summary],
                        Subjects = string.IsNullOrEmpty(dbBook.Subjects) ? [] : [dbBook.Subjects],
                        Languages = string.IsNullOrEmpty(dbBook.Language) ? ["en"] : [dbBook.Language],
                        Formats = new Dictionary<string, string>
                        {
                            ["image/jpeg"] = dbBook.CoverImage ?? ""
                        },
                        Download_Count = dbBook.DownloadCount
                    };
                }
                else
                {
                    return null;
                }
            }

            // Cache metadata in RAM for 2 hours
            _cache.Set(cacheKey, book, TimeSpan.FromHours(2));
            return book;
        }

        public async Task<string?> GetBookContent(int bookId, CancellationToken cancellationToken)
        {
            var cacheKey = $"book_content_{bookId}";
            if (_cache.TryGetValue(cacheKey, out string? cachedText) && !string.IsNullOrEmpty(cachedText))
            {
                return cachedText;
            }

            string bookText = string.Empty;

            // 1. First get book metadata to find working text formats
            var meta = await GetBook(new GetBookQuery(bookId), cancellationToken);
            var candidates = new List<string>();

            if (meta?.Formats != null)
            {
                if (meta.Formats.TryGetValue("text/plain; charset=utf-8", out var utf8Url) && !string.IsNullOrEmpty(utf8Url))
                    candidates.Add(utf8Url);
                if (meta.Formats.TryGetValue("text/plain; charset=us-ascii", out var asciiUrl) && !string.IsNullOrEmpty(asciiUrl))
                    candidates.Add(asciiUrl);
                if (meta.Formats.TryGetValue("text/plain", out var plainUrl) && !string.IsNullOrEmpty(plainUrl))
                    candidates.Add(plainUrl);
            }

            // Standard fallback Gutenberg cache URL
            candidates.Add($"https://www.gutenberg.org/cache/epub/{bookId}/pg{bookId}.txt");
            candidates.Add($"https://www.gutenberg.org/files/{bookId}/{bookId}-0.txt");

            foreach (var url in candidates.Distinct())
            {
                try
                {
                    using var request = new HttpRequestMessage(HttpMethod.Get, url);
                    var textResponse = await _gutenbergClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
                    if (textResponse.IsSuccessStatusCode)
                    {
                        var fetched = await textResponse.Content.ReadAsStringAsync(cancellationToken);
                        if (!string.IsNullOrWhiteSpace(fetched) && fetched.Length > 200)
                        {
                            bookText = fetched;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed fetching text from {url}: {ex.Message}");
                }
            }

            if (!string.IsNullOrEmpty(bookText))
            {
                _cache.Set(cacheKey, bookText, TimeSpan.FromHours(6));
            }

            return bookText;
        }

        public async Task<IEnumerable<Book>> GetAllBookAsync()
        {
            var books = await _dbContext.Books.ToListAsync();
            if (books == null)
            {
                throw new Exception("GetAllBookAsync returned null.");
            }
            return books;
        }
    }
}
