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
    public class FavoritesController : ControllerBase
    {
        private readonly AppDBContext _context;

        public FavoritesController(AppDBContext context)
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

        // GET: api/Favorites
        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = GetCurrentUserId();

            var favoriteBooks = await _context.Favorites
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.AddedAt)
                .Select(f => new
                {
                    f.Book.Id,
                    f.Book.GutendexId,
                    f.Book.Title,
                    f.Book.Author,
                    f.Book.CoverImage,
                    f.Book.Language,
                    f.Book.Genre,
                    f.Book.Summary,
                    f.Book.Subjects,
                    f.Book.DownloadCount,
                    f.AddedAt
                })
                .ToListAsync();

            return Ok(favoriteBooks);
        }

        // GET: api/Favorites/ids
        [HttpGet("ids")]
        public async Task<IActionResult> GetFavoriteBookIds()
        {
            var userId = GetCurrentUserId();
            var ids = await _context.Favorites
                .Where(f => f.UserId == userId)
                .Select(f => f.BookId)
                .ToListAsync();

            return Ok(ids);
        }

        // POST: api/Favorites/{bookId}
        [HttpPost("{bookId:long}")]
        public async Task<IActionResult> ToggleFavorite(long bookId)
        {
            var userId = GetCurrentUserId();

            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                return NotFound(new { message = "Book not found." });
            }

            var existing = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.BookId == bookId);

            if (existing != null)
            {
                _context.Favorites.Remove(existing);
                await _context.SaveChangesAsync();
                return Ok(new { isFavorite = false, message = "Removed from favorites." });
            }

            var favorite = new Favorite
            {
                UserId = userId,
                BookId = bookId,
                AddedAt = DateTime.UtcNow
            };

            await _context.Favorites.AddAsync(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { isFavorite = true, message = "Added to favorites." });
        }
    }
}
