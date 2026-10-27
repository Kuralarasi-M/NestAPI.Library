using System.Security.Claims;
using LibraryApp.Feature.Quiz.DTOs;
using LibraryApp.Feature.Quiz.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly IGamificationService _gamificationService;

        public QuizController(IQuizService quizService, IGamificationService gamificationService)
        {
            _quizService = quizService;
            _gamificationService = gamificationService;
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

        [HttpGet("Questions")]
        public async Task<IActionResult> GetQuestions([FromQuery] string genre = "Books", [FromQuery] string difficulty = "medium", [FromQuery] int amount = 5)
        {
            var questions = await _quizService.GetQuestionsAsync(genre, difficulty, amount);
            return Ok(questions);
        }

        [Authorize]
        [HttpPost("Submit")]
        public async Task<IActionResult> SubmitQuiz([FromBody] QuizSubmitRequest request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized("User ID not found in token.");

            var result = await _gamificationService.RecordQuizAttemptAsync(userId.Value, request);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("Stats")]
        public async Task<IActionResult> GetStats()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized("User ID not found in token.");

            var stats = await _gamificationService.GetUserStatsAsync(userId.Value);
            return Ok(stats);
        }

        [HttpGet("Leaderboard")]
        public async Task<IActionResult> GetLeaderboard([FromQuery] int top = 50)
        {
            var leaderboard = await _gamificationService.GetLeaderboardAsync(top);
            return Ok(leaderboard);
        }

        [Authorize]
        [HttpGet("Genres")]
        public async Task<IActionResult> GetUserGenres()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized("User ID not found in token.");

            var genres = await _gamificationService.GetUserGenresAsync(userId.Value);
            return Ok(genres);
        }

        [Authorize]
        [HttpPost("Genres")]
        public async Task<IActionResult> UpdateUserGenres([FromBody] SetGenresRequest request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized("User ID not found in token.");

            var success = await _gamificationService.UpdateUserGenresAsync(userId.Value, request.Genres);
            return Ok(new { success });
        }
    }
}
