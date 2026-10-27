using LibraryApp.Feature.WordGames.DTOs;

namespace LibraryApp.Feature.WordGames.Services
{
    public interface IWordGameService
    {
        List<SentenceBuilderGameDTO> GetSentenceBuilderGames();
        List<GuessTheWordGameDTO> GetGuessTheWordGames();
        WordSearchGameDTO GetWordSearchGame(string theme = "literature", int stage = 1);
    }

    public class WordGameService : IWordGameService
    {
        public List<SentenceBuilderGameDTO> GetSentenceBuilderGames()
        {
            return new List<SentenceBuilderGameDTO>
            {
                new SentenceBuilderGameDTO
                {
                    Id = 1,
                    Theme = "Reading Habit",
                    TargetSentence = "Every day of reading practice improves your skills",
                    ScrambledWords = new List<string> { "reading", "practice", "skills", "Every", "your", "improves", "day", "of" },
                    Hint = "Starts with 'Every'",
                    Points = 30
                },
                new SentenceBuilderGameDTO
                {
                    Id = 2,
                    Theme = "Knowledge",
                    TargetSentence = "Books are a uniquely portable magic that opens new worlds",
                    ScrambledWords = new List<string> { "new", "magic", "Books", "portable", "are", "uniquely", "worlds", "that", "a", "opens" },
                    Hint = "A famous quote by Stephen King",
                    Points = 35
                },
                new SentenceBuilderGameDTO
                {
                    Id = 3,
                    Theme = "Curiosity",
                    TargetSentence = "The mind is not a vessel to be filled but a fire to be kindled",
                    ScrambledWords = new List<string> { "vessel", "fire", "The", "not", "filled", "mind", "is", "a", "kindled", "to", "be", "but", "a" },
                    Hint = "Plutarch's wisdom on learning",
                    Points = 40
                },
                new SentenceBuilderGameDTO
                {
                    Id = 4,
                    Theme = "Vocabulary",
                    TargetSentence = "Expanding your vocabulary helps you express complex thoughts clearly",
                    ScrambledWords = new List<string> { "vocabulary", "thoughts", "express", "Expanding", "clearly", "complex", "you", "helps", "your" },
                    Hint = "Starts with 'Expanding'",
                    Points = 30
                },
                new SentenceBuilderGameDTO
                {
                    Id = 5,
                    Theme = "Dedication",
                    TargetSentence = "Patience and perseverance overcome the greatest obstacles in life",
                    ScrambledWords = new List<string> { "obstacles", "Patience", "life", "and", "greatest", "in", "the", "overcome", "perseverance" },
                    Hint = "Starts with 'Patience'",
                    Points = 35
                }
            };
        }

        public List<GuessTheWordGameDTO> GetGuessTheWordGames()
        {
            return new List<GuessTheWordGameDTO>
            {
                new GuessTheWordGameDTO
                {
                    Id = 1,
                    Word = "PERSISTENT",
                    Definition = "Continuing firmly in a course of action in spite of difficulty or opposition.",
                    PartOfSpeech = "Adjective",
                    Clues = new List<string>
                    {
                        "Synonym: determined, tenacious, unwavering.",
                        "First letter is 'P' and has 10 letters.",
                        "Tamil meaning: தளராமல் விடாப்பிடியாக இருக்கும் தன்மை."
                    },
                    BasePoints = 50
                },
                new GuessTheWordGameDTO
                {
                    Id = 2,
                    Word = "BENEVOLENT",
                    Definition = "Well meaning and kindly; serving a charitable rather than a profit-making purpose.",
                    PartOfSpeech = "Adjective",
                    Clues = new List<string>
                    {
                        "Synonym: kind-hearted, charitable, generous.",
                        "Opposite of malevolent.",
                        "Tamil meaning: கருணைமிக்க / நன்மை செய்யும்."
                    },
                    BasePoints = 50
                },
                new GuessTheWordGameDTO
                {
                    Id = 3,
                    Word = "RESILIENT",
                    Definition = "Able to withstand or recover quickly from difficult conditions.",
                    PartOfSpeech = "Adjective",
                    Clues = new List<string>
                    {
                        "Often used to describe people who bounce back after hardships.",
                        "Starts with 'R' and ends with 'T'.",
                        "Tamil meaning: இன்னல்களிலிருந்து மீண்டுவரும் திறன்."
                    },
                    BasePoints = 50
                },
                new GuessTheWordGameDTO
                {
                    Id = 4,
                    Word = "ELOQUENT",
                    Definition = "Fluent or persuasive in speaking or writing.",
                    PartOfSpeech = "Adjective",
                    Clues = new List<string>
                    {
                        "Characteristic of great public orators and speechmakers.",
                        "Has 8 letters and starts with 'E'.",
                        "Tamil meaning: நயம்படப் பேசும் ஆற்றல்."
                    },
                    BasePoints = 50
                },
                new GuessTheWordGameDTO
                {
                    Id = 5,
                    Word = "METICULOUS",
                    Definition = "Showing great attention to detail; very careful and precise.",
                    PartOfSpeech = "Adjective",
                    Clues = new List<string>
                    {
                        "Synonyms: thorough, diligent, scrupulous.",
                        "Starts with 'M' and has 10 letters.",
                        "Tamil meaning: மிகுந்த கவனமுடன் துல்லியமாக செயல்படும்."
                    },
                    BasePoints = 50
                }
            };
        }

        public WordSearchGameDTO GetWordSearchGame(string theme = "literature", int stage = 1)
        {
            if (stage < 1) stage = 1;

            // Determine difficulty tier
            string difficulty = stage switch
            {
                <= 2 => "Easy",
                <= 4 => "Medium",
                <= 6 => "Hard",
                _ => "Master"
            };

            // Grid size scales with difficulty: Easy=8, Medium=10, Hard=12, Master=12
            int size = stage switch
            {
                1 => 8,
                2 => 8,
                3 => 10,
                4 => 10,
                5 => 12,
                _ => 12
            };

            // Words pool based on theme & difficulty
            var words = (theme.ToLowerInvariant(), difficulty) switch
            {
                ("nature", "Easy") => new List<string> { "TREE", "RAIN", "WIND", "LEAF", "SUN" },
                ("nature", "Medium") => new List<string> { "FOREST", "RIVER", "OCEAN", "BREEZE", "DESERT", "CANYON" },
                ("nature", "Hard") => new List<string> { "WATERFALL", "MOUNTAIN", "GLACIER", "VOLCANO", "WILDLIFE", "HORIZON" },
                ("nature", "Master") => new List<string> { "BIODIVERSITY", "RAINFOREST", "ECOSYSTEM", "ATMOSPHERE", "WILDERNESS", "LANDSCAPE" },

                ("science", "Easy") => new List<string> { "ATOM", "MOON", "HEAT", "CELL", "STAR" },
                ("science", "Medium") => new List<string> { "ENERGY", "GALAXY", "QUANTUM", "GRAVITY", "PLANET", "NEURON" },
                ("science", "Hard") => new List<string> { "ASTEROID", "VELOCITY", "RADIATION", "MOLECULE", "ORGANISM", "SPECTRUM" },
                ("science", "Master") => new List<string> { "ASTROPHYSICS", "THERMODYNAMICS", "CHROMOSOME", "ELECTROMAGNET", "ACCELERATION" },

                ("literature", "Easy") => new List<string> { "BOOK", "PAGE", "READ", "TALE", "POEM" },
                ("literature", "Medium") => new List<string> { "POETRY", "NOVEL", "AUTHOR", "CHAPTER", "STORY", "LIBRARY" },
                ("literature", "Hard") => new List<string> { "METAPHOR", "CHARACTER", "DIALOGUE", "PROTAGONIST", "NARRATIVE" },
                _ => new List<string> { "ALLITERATION", "FORESHADOWING", "MANUSCRIPT", "BIBLIOGRAPHY", "ANTHOLOGY" }
            };

            int points = stage switch
            {
                <= 2 => 40 + (stage * 10),
                <= 4 => 70 + (stage * 10),
                _ => 120 + (stage * 15)
            };

            char[,] grid = new char[size, size];
            var rand = new Random(theme.GetHashCode() + stage * 31);

            // Initialize with spaces
            for (int r = 0; r < size; r++)
                for (int c = 0; c < size; c++)
                    grid[r, c] = ' ';

            // Allowed directions:
            // Easy: Horizontal (0)
            // Medium: Horizontal (0), Vertical (1)
            // Hard/Master: Horizontal (0), Vertical (1), Diagonal (2)
            int directionCount = stage <= 2 ? 1 : (stage <= 4 ? 2 : 3);

            // Place words
            foreach (var word in words)
            {
                bool placed = false;
                int attempts = 0;
                while (!placed && attempts < 150)
                {
                    attempts++;
                    int dir = rand.Next(directionCount); // 0 = horizontal, 1 = vertical, 2 = diagonal down-right

                    int dr = dir == 0 ? 0 : 1;
                    int dc = dir == 1 ? 0 : 1;

                    int maxR = size - (dr * word.Length);
                    int maxC = size - (dc * word.Length);

                    if (maxR <= 0 || maxC <= 0) continue;

                    int r = rand.Next(maxR);
                    int c = rand.Next(maxC);

                    bool canPlace = true;
                    for (int i = 0; i < word.Length; i++)
                    {
                        int checkR = r + (dr * i);
                        int checkC = c + (dc * i);
                        if (grid[checkR, checkC] != ' ' && grid[checkR, checkC] != word[i])
                        {
                            canPlace = false;
                            break;
                        }
                    }

                    if (canPlace)
                    {
                        for (int i = 0; i < word.Length; i++)
                        {
                            int placeR = r + (dr * i);
                            int placeC = c + (dc * i);
                            grid[placeR, placeC] = word[i];
                        }
                        placed = true;
                    }
                }
            }

            // Fill empty cells with random letters
            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    if (grid[r, c] == ' ')
                    {
                        grid[r, c] = (char)('A' + rand.Next(26));
                    }
                }
            }

            var gridList = new List<List<string>>();
            for (int r = 0; r < size; r++)
            {
                var row = new List<string>();
                for (int c = 0; c < size; c++)
                {
                    row.Add(grid[r, c].ToString());
                }
                gridList.Add(row);
            }

            return new WordSearchGameDTO
            {
                Id = stage,
                Theme = char.ToUpper(theme[0]) + theme.Substring(1),
                Stage = stage,
                Difficulty = difficulty,
                GridSize = size,
                Grid = gridList,
                WordsToFind = words,
                Points = points
            };
        }
    }
}
