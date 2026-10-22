using System.Security.Claims;
using LibraryApp.Data;
using LibraryApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReadingHistoryController : ControllerBase
    {
        private readonly AppDBContext _context;

        public ReadingHistoryController(AppDBContext context)
        {
            _context = context;
        }

        private Guid GetCurrentUserId()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            {
                throw new UnauthorizedAccessException("User identifier claim is missing or invalid.");
            }
            return userId;
        }

        // GET: api/ReadingHistory
        [HttpGet]
        public async Task<IActionResult> GetReadingHistory()
        {
            var userId = GetCurrentUserId();

            var history = await _context.ReadingHistories
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.LastReadAt)
                .Select(r => new
                {
                    r.Id,
                    r.LastReadAt,
                    Book = new
                    {
                        r.Book.Id,
                        r.Book.GutendexId,
                        r.Book.Title,
                        r.Book.Author,
                        r.Book.CoverImage,
                        r.Book.Language,
                        r.Book.Genre,
                        r.Book.Summary,
                        r.Book.Subjects,
                        r.Book.DownloadCount
                    }
                })
                .ToListAsync();

            return Ok(history);
        }

        // POST: api/ReadingHistory/{bookId}
        [HttpPost("{bookId:long}")]
        public async Task<IActionResult> RecordReading(long bookId)
        {
            var userId = GetCurrentUserId();

            // Book can be looked up by internal Id or GutendexId
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId || b.GutendexId == (int)bookId);
            if (book == null)
            {
                return NotFound(new { message = "Book not found." });
            }

            var existing = await _context.ReadingHistories
                .FirstOrDefaultAsync(r => r.UserId == userId && r.BookId == book.Id);

            if (existing != null)
            {
                existing.LastReadAt = DateTime.UtcNow;
                _context.ReadingHistories.Update(existing);
            }
            else
            {
                var record = new ReadingHistory
                {
                    UserId = userId,
                    BookId = book.Id,
                    LastReadAt = DateTime.UtcNow
                };
                await _context.ReadingHistories.AddAsync(record);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Reading history recorded successfully." });
        }
    }
}
