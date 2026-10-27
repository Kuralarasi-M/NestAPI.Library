using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class DailyTamilSentence
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Word { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Phonetic { get; set; } = string.Empty;

        [MaxLength(50)]
        public string PartOfSpeech { get; set; } = "Noun";

        [Required]
        [MaxLength(200)]
        public string TamilMeaning { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string EnglishSentence { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string TamilSentence { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Scenario { get; set; } = string.Empty;

        public int DayNumber { get; set; } // 1 to 365, or cycle index
    }
}
