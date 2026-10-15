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
            // 1. Get book details from Gutendex
            var response = await _httpClient.GetAsync(
                $"books/{gutendexId.BookId}",
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(
                cancellationToken);

            var book = JsonSerializer.Deserialize<GetBookResponseDTO>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (book == null)
                return null;

            // 2. Download the actual book text from Gutenberg
            var textUrl =
                $"https://www.gutenberg.org/cache/epub/{gutendexId.BookId}/pg{gutendexId.BookId}.txt";

            var textResponse = await _gutenbergClient.GetAsync(
                textUrl,
                cancellationToken);

            textResponse.EnsureSuccessStatusCode();

            var bookText = await textResponse.Content.ReadAsStringAsync(
                cancellationToken);

            // 3. Add the text to the response
            book.BookText = bookText;

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
