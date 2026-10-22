using LibraryApp.Data;
using LibraryApp.Feature.Auth.Commands;
using LibraryApp.Feature.Auth.Handler;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly AppDBContext _context;

        public BooksController(IMediator mediator, AppDBContext context)
        {
            _mediator = mediator;
            _context = context;
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
