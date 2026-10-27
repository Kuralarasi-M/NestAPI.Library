namespace LibraryApp.Feature.WordGames.DTOs
{
    public class SentenceBuilderGameDTO
    {
        public int Id { get; set; }
        public string Theme { get; set; } = string.Empty;
        public string TargetSentence { get; set; } = string.Empty;
        public List<string> ScrambledWords { get; set; } = new();
        public string Hint { get; set; } = string.Empty;
        public int Points { get; set; } = 30;
    }

    public class GuessTheWordGameDTO
    {
        public int Id { get; set; }
        public string Word { get; set; } = string.Empty;
        public string Definition { get; set; } = string.Empty;
        public string PartOfSpeech { get; set; } = string.Empty;
        public List<string> Clues { get; set; } = new();
        public int MaxHints { get; set; } = 3;
        public int BasePoints { get; set; } = 50;
    }

    public class WordSearchGameDTO
    {
        public int Id { get; set; }
        public string Theme { get; set; } = string.Empty;
        public int Stage { get; set; } = 1;
        public string Difficulty { get; set; } = "Easy";
        public int GridSize { get; set; } = 8;
        public List<List<string>> Grid { get; set; } = new();
        public List<string> WordsToFind { get; set; } = new();
        public int Points { get; set; } = 60;
    }

    public class SubmitGameScoreRequest
    {
        public string GameType { get; set; } = string.Empty; // "sentence-builder", "guess-word", "word-search"
        public int PointsEarned { get; set; }
        public string Details { get; set; } = string.Empty;
        public int? CompletedStage { get; set; }
    }

    public class SubmitGameScoreResponse
    {
        public int PointsEarned { get; set; }
        public int TotalPoints { get; set; }
        public int WordSearchStage { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
