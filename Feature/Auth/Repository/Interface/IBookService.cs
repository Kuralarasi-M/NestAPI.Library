using LibraryApp.Feature.Auth.Commands;
using LibraryApp.Models;
using System.Collections;

namespace LibraryApp.Feature.Auth.Repository.Interface
{
    public interface IBookService
    {
        Task<GetBookResponseDTO?> GetBook(GetBookQuery getBook, CancellationToken cancellationToken);
        Task<string?> GetBookContent(int bookId, CancellationToken cancellationToken);
        Task<IEnumerable<Book>> GetAllBookAsync();
    }
}

