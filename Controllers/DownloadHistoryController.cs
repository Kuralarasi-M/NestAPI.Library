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
    public class DownloadHistoryController : ControllerBase
    {
        private readonly AppDBContext _context;

        public DownloadHistoryController(AppDBContext context)
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

        // GET: api/DownloadHistory
        [HttpGet]
        public async Task<IActionResult> GetDownloadHistory()
        {
            var userId = GetCurrentUserId();

            var history = await _context.DownloadHistories
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.DownloadedAt)
                .Select(d => new
                {
                    d.Id,
                    d.DownloadedAt,
                    Book = new
                    {
                        d.Book.Id,
                        d.Book.GutendexId,
                        d.Book.Title,
                        d.Book.Author,
                        d.Book.CoverImage,
                        d.Book.Language,
                        d.Book.Genre,
                        d.Book.Summary,
                        d.Book.Subjects,
                        d.Book.DownloadCount
                    }
                })
                .ToListAsync();

            return Ok(history);
        }

        // POST: api/DownloadHistory/{bookId}
        [HttpPost("{bookId:long}")]
        public async Task<IActionResult> RecordDownload(long bookId)
        {
            var userId = GetCurrentUserId();

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId || b.GutendexId == (int)bookId);
            if (book == null)
            {
                return NotFound(new { message = "Book not found." });
            }

            var record = new DownloadHistory
            {
                UserId = userId,
                BookId = book.Id,
                DownloadedAt = DateTime.UtcNow
            };

            await _context.DownloadHistories.AddAsync(record);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Download history recorded successfully." });
        }
    }
}
