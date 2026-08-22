using LibraryApp.Feature.Auth.Commands;
using LibraryApp.Feature.Auth.Repository.Interface;
using MediatR;
using LibraryApp.Data;
namespace LibraryApp.Feature.Auth.Handler
{
    public class GetBookHandler : IRequestHandler<GetBookQuery, GetBookResponseDTO>
    {
        private readonly IBookService _bookService;
        public GetBookHandler(IBookService bookService)
        {
            _bookService = bookService;
        }
        public async Task<GetBookResponseDTO> Handle(GetBookQuery getBook,CancellationToken token)
        {
            var book =await  _bookService.GetBook(getBook, token);
            GetBookResponseDTO bookres = new GetBookResponseDTO();
            return book;
        }
    }
}
