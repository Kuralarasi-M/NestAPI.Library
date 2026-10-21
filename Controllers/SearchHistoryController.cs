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
    public class SearchHistoryController : ControllerBase
    {
        private readonly AppDBContext _context;

        public SearchHistoryController(AppDBContext context)
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

        // GET: api/SearchHistory
        [HttpGet]
        public async Task<IActionResult> GetSearchHistory()
        {
            var userId = GetCurrentUserId();

            var searches = await _context.SearchHistories
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.SearchAt)
                .Take(30)
                .Select(s => new
                {
                    s.Id,
                    s.Query,
                    s.SearchAt
                })
                .ToListAsync();

            return Ok(searches);
        }

        // POST: api/SearchHistory
        [HttpPost]
        public async Task<IActionResult> RecordSearch([FromBody] SearchReq req)
        {
            if (string.IsNullOrWhiteSpace(req?.Query))
            {
                return BadRequest("Search query cannot be empty.");
            }

            var userId = GetCurrentUserId();

            // Default to first book in DB or placeholder to satisfy DB FK if present
            var anyBook = await _context.Books.Select(b => b.Id).FirstOrDefaultAsync();

            var item = new SearchHistory
            {
                UserId = userId,
                Query = req.Query.Trim(),
                SearchAt = DateTime.UtcNow,
                Book = anyBook != 0 ? await _context.Books.FindAsync(anyBook) : null!
            };

            await _context.SearchHistories.AddAsync(item);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Search query saved." });
        }

        // DELETE: api/SearchHistory
        [HttpDelete]
        public async Task<IActionResult> ClearSearchHistory()
        {
            var userId = GetCurrentUserId();

            var items = await _context.SearchHistories
                .Where(s => s.UserId == userId)
                .ToListAsync();

            _context.SearchHistories.RemoveRange(items);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Search history cleared." });
        }
    }

    public class SearchReq
    {
        public string Query { get; set; } = string.Empty;
    }
}
