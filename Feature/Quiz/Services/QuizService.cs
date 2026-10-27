using System.Net;
using System.Text.Json;
using System.Web;
using LibraryApp.Feature.Quiz.DTOs;
using Microsoft.Extensions.Caching.Memory;

namespace LibraryApp.Feature.Quiz.Services
{
    public interface IQuizService
    {
        Task<List<QuizQuestionDTO>> GetQuestionsAsync(string genre, string difficulty, int amount = 5);
    }

    public class QuizService : IQuizService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<QuizService> _logger;

        public QuizService(HttpClient httpClient, IMemoryCache cache, ILogger<QuizService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        private static int MapGenreToOpenTdbCategory(string genre)
        {
            return genre?.Trim().ToLowerInvariant() switch
            {
                "books" or "literature" or "poetry" or "fiction" => 10, // Entertainment: Books
                "film" or "cinema" or "movies" => 11,
                "music" => 12,
                "science" or "nature" or "biology" or "physics" => 17, // Science & Nature
                "computers" or "technology" or "programming" => 18, // Science: Computers
                "mathematics" or "math" => 19,
                "mythology" => 20,
                "sports" => 21,
                "geography" => 22,
                "history" => 23,
                "politics" => 24,
                "art" => 25,
                "celebrities" => 26,
                "animals" => 27,
                _ => 10 // default to Books
            };
        }

        public async Task<List<QuizQuestionDTO>> GetQuestionsAsync(string genre, string difficulty, int amount = 5)
        {
            var categoryId = MapGenreToOpenTdbCategory(genre);
            var normalizedDifficulty = string.IsNullOrWhiteSpace(difficulty) ? "medium" : difficulty.ToLowerInvariant();
            var cacheKey = $"quiz_{categoryId}_{normalizedDifficulty}_{amount}";

            if (_cache.TryGetValue(cacheKey, out List<QuizQuestionDTO>? cached) && cached != null && cached.Count > 0)
            {
                return cached;
            }

            try
            {
                var url = $"https://opentdb.com/api.php?amount={amount}&category={categoryId}&difficulty={normalizedDifficulty}&type=multiple";
                var response = await _httpClient.GetFromJsonAsync<OpenTdbResponse>(url);

                if (response != null && response.Results != null && response.Results.Count > 0)
                {
                    var questions = new List<QuizQuestionDTO>();
                    int idx = 1;
                    var rand = new Random();

                    foreach (var item in response.Results)
                    {
                        var decodedQuestion = HttpUtility.HtmlDecode(item.Question);
                        var decodedCorrect = HttpUtility.HtmlDecode(item.CorrectAnswer);
                        var decodedOptions = item.IncorrectAnswers.Select(HttpUtility.HtmlDecode).ToList();

                        // Insert correct answer randomly
                        int correctIndex = rand.Next(0, decodedOptions.Count + 1);
                        decodedOptions.Insert(correctIndex, decodedCorrect);

                        questions.Add(new QuizQuestionDTO
                        {
                            Id = idx++,
                            Category = item.Category,
                            Difficulty = item.Difficulty,
                            Question = decodedQuestion,
                            Options = decodedOptions,
                            CorrectOptionIndex = correctIndex
                        });
                    }

                    _cache.Set(cacheKey, questions, TimeSpan.FromMinutes(10));
                    return questions;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch questions from OpenTDB. Using fallback questions.");
            }

            // Fallback questions if external API is unreachable or rate limited
            return GetFallbackQuestions(genre, normalizedDifficulty);
        }

        private static List<QuizQuestionDTO> GetFallbackQuestions(string genre, string difficulty)
        {
            return new List<QuizQuestionDTO>
            {
                new QuizQuestionDTO
                {
                    Id = 1,
                    Category = genre,
                    Difficulty = difficulty,
                    Question = $"In classic literature, who wrote 'Pride and Prejudice'?",
                    Options = new List<string> { "Jane Austen", "Emily Brontë", "Virginia Woolf", "Mary Shelley" },
                    CorrectOptionIndex = 0
                },
                new QuizQuestionDTO
                {
                    Id = 2,
                    Category = genre,
                    Difficulty = difficulty,
                    Question = "What is the title of the famous dystopian novel written by George Orwell published in 1949?",
                    Options = new List<string> { "Brave New World", "1984", "Fahrenheit 451", "Animal Farm" },
                    CorrectOptionIndex = 1
                },
                new QuizQuestionDTO
                {
                    Id = 3,
                    Category = genre,
                    Difficulty = difficulty,
                    Question = "Which ancient Greek epic poem follows Odysseus on his journey home?",
                    Options = new List<string> { "The Iliad", "The Aeneid", "The Odyssey", "Metamorphoses" },
                    CorrectOptionIndex = 2
                },
                new QuizQuestionDTO
                {
                    Id = 4,
                    Category = genre,
                    Difficulty = difficulty,
                    Question = "Who is the creator of the famous detective Sherlock Holmes?",
                    Options = new List<string> { "Agatha Christie", "Arthur Conan Doyle", "Edgar Allan Poe", "Raymond Chandler" },
                    CorrectOptionIndex = 1
                },
                new QuizQuestionDTO
                {
                    Id = 5,
                    Category = genre,
                    Difficulty = difficulty,
                    Question = "Which author wrote 'The Great Gatsby'?",
                    Options = new List<string> { "F. Scott Fitzgerald", "Ernest Hemingway", "John Steinbeck", "Mark Twain" },
                    CorrectOptionIndex = 0
                }
            };
        }
    }
}
