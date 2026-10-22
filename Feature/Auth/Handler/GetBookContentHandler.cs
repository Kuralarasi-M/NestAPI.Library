using LibraryApp.Feature.Auth.Commands;
using LibraryApp.Feature.Auth.Repository.Interface;
using MediatR;

namespace LibraryApp.Feature.Auth.Handler
{
    public class GetBookContentHandler : IRequestHandler<GetBookContentQuery, BookContentResponseDTO>
    {
        private readonly IBookService _bookService;

        public GetBookContentHandler(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<BookContentResponseDTO> Handle(GetBookContentQuery request, CancellationToken cancellationToken)
        {
            var content = await _bookService.GetBookContent(request.BookId, cancellationToken);
            return new BookContentResponseDTO
            {
                BookId = request.BookId,
                BookText = content ?? string.Empty
            };
        }
    }
}
