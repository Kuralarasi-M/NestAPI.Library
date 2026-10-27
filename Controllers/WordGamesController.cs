using System.Security.Claims;
using LibraryApp.Data;
using LibraryApp.Feature.WordGames.DTOs;
using LibraryApp.Feature.WordGames.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WordGamesController : ControllerBase
    {
        private readonly IWordGameService _wordGameService;
        private readonly AppDBContext _context;

        public WordGamesController(IWordGameService wordGameService, AppDBContext context)
        {
            _wordGameService = wordGameService;
            _context = context;
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("nameid")?.Value
                ?? User.FindFirst("userId")?.Value;

            if (Guid.TryParse(userIdClaim, out var id))
            {
                return id;
            }
            return null;
        }

        [HttpGet("SentenceBuilder")]
        public IActionResult GetSentenceBuilder()
        {
            var games = _wordGameService.GetSentenceBuilderGames();
            return Ok(games);
        }

        [HttpGet("GuessTheWord")]
        public IActionResult GetGuessTheWord()
        {
            var games = _wordGameService.GetGuessTheWordGames();
            return Ok(games);
        }

        [HttpGet("WordSearch")]
        public async Task<IActionResult> GetWordSearch([FromQuery] string theme = "literature", [FromQuery] int? stage = null)
        {
            var userId = GetCurrentUserId();
            int currentStage = stage ?? 1;

            if (userId.HasValue && !stage.HasValue)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);
                if (user != null && user.WordSearchStage > 0)
                {
                    currentStage = user.WordSearchStage;
                }
            }

            var game = _wordGameService.GetWordSearchGame(theme, currentStage);
            return Ok(game);
        }

        [HttpGet("UserProgress")]
        public async Task<IActionResult> GetUserProgress()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Ok(new { wordSearchStage = 1, totalPoints = 0 });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);
            return Ok(new
            {
                wordSearchStage = user?.WordSearchStage ?? 1,
                totalPoints = user?.TotalPoints ?? 0
            });
        }

        [Authorize]
        [HttpPost("SubmitScore")]
        public async Task<IActionResult> SubmitScore([FromBody] SubmitGameScoreRequest request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized("User not identified.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);
            if (user == null) return NotFound("User not found.");

            user.TotalPoints += request.PointsEarned;

            if (request.CompletedStage.HasValue && request.CompletedStage.Value >= user.WordSearchStage)
            {
                user.WordSearchStage = request.CompletedStage.Value + 1;
            }

            await _context.SaveChangesAsync();

            return Ok(new SubmitGameScoreResponse
            {
                PointsEarned = request.PointsEarned,
                TotalPoints = user.TotalPoints,
                WordSearchStage = user.WordSearchStage,
                Message = $"Great job! Added {request.PointsEarned} points to your profile."
            });
        }
    }
}
