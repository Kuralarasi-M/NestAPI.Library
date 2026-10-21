using MediatR;

namespace LibraryApp.Feature.Auth.Commands
{
    public class GetBookContentQuery : IRequest<BookContentResponseDTO>
    {
        public int BookId { get; }

        public GetBookContentQuery(int bookId)
        {
            BookId = bookId;
        }
    }

    public class BookContentResponseDTO
    {
        public int BookId { get; set; }
        public string BookText { get; set; } = string.Empty;
    }
}
