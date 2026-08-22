using LibraryApp.Data;
using LibraryApp.Feature.Auth.Commands;
using LibraryApp.Feature.Auth.Handler;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BooksController(IMediator mediator)
        {
            _mediator = mediator;
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
}}
