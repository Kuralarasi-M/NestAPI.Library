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
        private readonly AppDBContext _dbContext;

        public BookService(HttpClient httpClient,AppDBContext context)
        {
            _httpClient = httpClient;
            _dbContext = context;
        }

        public async Task<GetBookResponseDTO> GetBook(GetBookQuery gutendexId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(
                $"books/{gutendexId.BookId}",
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            var book = JsonSerializer.Deserialize<GetBookResponseDTO>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (book == null)
                return null;

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
