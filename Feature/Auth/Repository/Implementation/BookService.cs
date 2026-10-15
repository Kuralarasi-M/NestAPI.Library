using LibraryApp.Data;
using LibraryApp.Feature.Auth.Commands;
using LibraryApp.Feature.Auth.Repository.Interface;
using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LibraryApp.Feature.Auth.Repository.Implementation
{
    public class BookService : IBookService
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClient _gutenbergClient;
        private readonly AppDBContext _dbContext;
        public BookService(
    HttpClient httpClient,
    AppDBContext context,
    IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClient;
            _dbContext = context;
            _gutenbergClient = httpClientFactory.CreateClient("Gutenberg");
        }

        public async Task<GetBookResponseDTO> GetBook(
     GetBookQuery gutendexId,
     CancellationToken cancellationToken)
        {
            GetBookResponseDTO? book = null;
            try
            {
                // 1. Get book details from Gutendex (ensure trailing slash)
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

            // 2. Download the actual book text from Gutenberg with fallback
            var textUrl =
                $"https://www.gutenberg.org/cache/epub/{gutendexId.BookId}/pg{gutendexId.BookId}.txt";

            try
            {
                var textResponse = await _gutenbergClient.GetAsync(
                    textUrl,
                    HttpCompletionOption.ResponseContentRead,
                    cancellationToken);

                if (textResponse.IsSuccessStatusCode)
                {
                    book.BookText = await textResponse.Content.ReadAsStringAsync(cancellationToken);
                }
            }
            catch (Exception)
            {
                // If Gutenberg drops connection or fails, fall back to any available format or empty string
                if (book.Formats != null && book.Formats.TryGetValue("text/plain; charset=utf-8", out var plainUrl))
                {
                    try
                    {
                        var fallbackResponse = await _gutenbergClient.GetAsync(plainUrl, cancellationToken);
                        if (fallbackResponse.IsSuccessStatusCode)
                        {
                            book.BookText = await fallbackResponse.Content.ReadAsStringAsync(cancellationToken);
                        }
                    }
                    catch
                    {
                        book.BookText = string.Empty;
                    }
                }
                else
                {
                    book.BookText = string.Empty;
                }
            }

            return book;
        }
        public async Task<IEnumerable<Book>> GetAllBookAsync()
        {
            var books= await _dbContext.Books.ToListAsync();
            if (books == null)
            {
                throw new Exception("GetAllBookAsync returned null.");
            }
            return books;
        }
    }
}
