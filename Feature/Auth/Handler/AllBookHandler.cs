using LibraryApp.Feature.Auth.Commands;
using LibraryApp.Feature.Auth.Repository.Interface;
using MediatR;

namespace LibraryApp.Feature.Auth.Handler
{
    public class AllBookHandler : IRequestHandler<AllBooksQuery, IEnumerable<BooksResponse>>
    {
        private readonly IBookService service;
        public AllBookHandler(IBookService service)
        {
            this.service = service;
        }
        public async Task<IEnumerable<BooksResponse>> Handle(AllBooksQuery query, CancellationToken cancellationToken)
        {
            var book = await service.GetAllBookAsync();
            var books = book.Select(book => new BooksResponse
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
                Author= book.Author
            }).ToList();
            return books;
        }
    }
}
