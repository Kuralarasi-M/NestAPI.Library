using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection.Emit;

namespace LibraryApp.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> option) : base(option)
        { }
        public DbSet<User> Users => Set<User>();

        public DbSet<Book> Books => Set<Book>();

        public DbSet<Favorite> Favorites => Set<Favorite>();

        public DbSet<ReadingHistory> ReadingHistories => Set<ReadingHistory>();

        public DbSet<DownloadHistory> DownloadHistories => Set<DownloadHistory>();

        public DbSet<SearchHistory> SearchHistories => Set<SearchHistory>();
        public DbSet<UserFcmToken> UserFcmTokens { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserGenre> UserGenres => Set<UserGenre>();
        public DbSet<UserQuizAttempt> UserQuizAttempts => Set<UserQuizAttempt>();
        public DbSet<Badge> Badges => Set<Badge>();
        public DbSet<UserBadge> UserBadges => Set<UserBadge>();
        public DbSet<BookReview> BookReviews => Set<BookReview>();
        public DbSet<DailyReadLog> DailyReadLogs => Set<DailyReadLog>();
        public DbSet<DailyTamilSentence> DailyTamilSentences => Set<DailyTamilSentence>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Relationships
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<ReadingHistory>()
                .HasOne(r => r.User)
                .WithMany(u => u.ReadingHistories)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<SearchHistory>()
                .HasOne(s => s.User)
                .WithMany(u => u.SearchHistories)
                .HasForeignKey(s => s.UserId);

            modelBuilder.Entity<UserFcmToken>()
                .HasOne(x => x.User)
                .WithMany(u => u.FcmTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserGenre>()
                .HasOne(g => g.User)
                .WithMany(u => u.SelectedGenres)
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserGenre>()
                .HasIndex(g => new { g.UserId, g.Genre })
                .IsUnique();

            modelBuilder.Entity<UserQuizAttempt>()
                .HasOne(q => q.User)
                .WithMany(u => u.QuizAttempts)
                .HasForeignKey(q => q.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserBadge>()
                .HasOne(ub => ub.User)
                .WithMany(u => u.Badges)
                .HasForeignKey(ub => ub.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserBadge>()
                .HasOne(ub => ub.Badge)
                .WithMany()
                .HasForeignKey(ub => ub.BadgeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookReview>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookReview>()
                .HasIndex(r => r.BookId);

            modelBuilder.Entity<DailyReadLog>()
                .HasOne(d => d.User)
                .WithMany(u => u.DailyReadLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DailyReadLog>()
                .HasIndex(d => new { d.UserId, d.CompletedDate })
                .IsUnique();

            // Seed default Badges
            modelBuilder.Entity<Badge>().HasData(
                new Badge { Id = 1, Name = "Quick Starter", Description = "Completed your first quiz", Icon = "Zap", RequiredPoints = 10, RequiredStreak = 0 },
                new Badge { Id = 2, Name = "Streak Starter", Description = "Reached a 3-day reading streak", Icon = "Flame", RequiredPoints = 0, RequiredStreak = 3 },
                new Badge { Id = 3, Name = "Dedicated Reader", Description = "Reached a 7-day reading streak", Icon = "Flame", RequiredPoints = 0, RequiredStreak = 7 },
                new Badge { Id = 4, Name = "Trivia Novice", Description = "Earned 100 quiz points", Icon = "Award", RequiredPoints = 100, RequiredStreak = 0 },
                new Badge { Id = 5, Name = "Scholar", Description = "Earned 500 quiz points", Icon = "BookOpen", RequiredPoints = 500, RequiredStreak = 0 },
                new Badge { Id = 6, Name = "Master Mind", Description = "Earned 1000 quiz points", Icon = "Trophy", RequiredPoints = 1000, RequiredStreak = 0 }
            );

            // Seed initial 15 English-Tamil daily sentences with scenarios and meanings
            modelBuilder.Entity<DailyTamilSentence>().HasData(
                new DailyTamilSentence
                {
                    Id = 1,
                    DayNumber = 1,
                    Word = "Perseverance",
                    Phonetic = "/ˌpɜːsɪˈvɪərəns/",
                    PartOfSpeech = "Noun",
                    TamilMeaning = "விடாமுயற்சி",
                    EnglishSentence = "Success in any competitive exam requires patience and perseverance.",
                    TamilSentence = "போட்டித் தேர்வுகளில் வெற்றி பெற பொறுமையும் விடாமுயற்சியும் அவசியம்.",
                    Scenario = "Career & Exam Preparation (வேலை & கல்வி சூழல்)"
                },
                new DailyTamilSentence
                {
                    Id = 2,
                    DayNumber = 1,
                    Word = "Enthusiastic",
                    Phonetic = "/ɪnˌθjuːziˈæstɪk/",
                    PartOfSpeech = "Adjective",
                    TamilMeaning = "ஆர்வமிக்க / உற்சாகமான",
                    EnglishSentence = "The students were very enthusiastic about the new science project.",
                    TamilSentence = "புதிய அறிவியல் திட்டத்தில் மாணவர்கள் மிகவும் ஆர்வமாகவும் உற்சாகமாகவும் இருந்தனர்.",
                    Scenario = "School & Learning (பள்ளி மற்றும் கற்றல் சூழல்)"
                },
                new DailyTamilSentence
                {
                    Id = 3,
                    DayNumber = 1,
                    Word = "Magnificent",
                    Phonetic = "/mæɡˈnɪfɪsnt/",
                    PartOfSpeech = "Adjective",
                    TamilMeaning = "அற்புதமான / கம்பீரமான",
                    EnglishSentence = "The ancient Tanjore temple has a magnificent architecture.",
                    TamilSentence = "பழங்கால தஞ்சைப் பெரிய கோவில் அற்புதமான கட்டடக்கலை கொண்டது.",
                    Scenario = "Travel & Culture (சுற்றுலா & கலாச்சாரம்)"
                },
                new DailyTamilSentence
                {
                    Id = 4,
                    DayNumber = 1,
                    Word = "Gratitude",
                    Phonetic = "/ˈɡrætɪtjuːd/",
                    PartOfSpeech = "Noun",
                    TamilMeaning = "நன்றி உணர்வு",
                    EnglishSentence = "He expressed his sincere gratitude to everyone who helped him.",
                    TamilSentence = "தனக்கு உதவிய அனைவருக்கும் அவர் தனது உளமார்ந்த நன்றி உணர்வைத் தெரிவித்தார்.",
                    Scenario = "Social Courtesy (சமூக நற்பண்பு)"
                },
                new DailyTamilSentence
                {
                    Id = 5,
                    DayNumber = 1,
                    Word = "Resilient",
                    Phonetic = "/rɪˈzɪliənt/",
                    PartOfSpeech = "Adjective",
                    TamilMeaning = "மீண்டுவரும் தன்மை கொண்ட / தளராத",
                    EnglishSentence = "She remained resilient despite facing numerous unexpected difficulties.",
                    TamilSentence = "பல எதிர்பாராத இன்னல்களை சந்தித்த போதிலும், அவள் தளராமல் மீண்டு வந்தாள்.",
                    Scenario = "Overcoming Obstacles (சவால்களை எதிர்கொள்ளுதல்)"
                },
                new DailyTamilSentence
                {
                    Id = 6,
                    DayNumber = 2,
                    Word = "Curiosity",
                    Phonetic = "/ˌkjʊəriˈɒsəti/",
                    PartOfSpeech = "Noun",
                    TamilMeaning = "அறியும் ஆர்வம் / புதுமை நாட்டம்",
                    EnglishSentence = "Children learn quickly because of their natural curiosity.",
                    TamilSentence = "இயற்கையான அறியும் ஆர்வத்தின் காரணமாக குழந்தைகள் விரைவாகக் கற்றுக்கொள்கிறார்கள்.",
                    Scenario = "Everyday Life (அன்றாட வாழ்க்கை)"
                },
                new DailyTamilSentence
                {
                    Id = 7,
                    DayNumber = 2,
                    Word = "Punctual",
                    Phonetic = "/ˈpʌŋktʃuəl/",
                    PartOfSpeech = "Adjective",
                    TamilMeaning = "நேரந்தவறாமை",
                    EnglishSentence = "Being punctual to morning meetings shows your professional dedication.",
                    TamilSentence = "காலை கூட்டங்களுக்கு நேரந்தவறாமல் வருவது உங்கள் தொழில் அர்ப்பணிப்பைக் காட்டுகிறது.",
                    Scenario = "Workplace & Office (அலுவலகச் சூழல்)"
                },
                new DailyTamilSentence
                {
                    Id = 8,
                    DayNumber = 2,
                    Word = "Compassion",
                    Phonetic = "/kəmˈpæʃn/",
                    PartOfSpeech = "Noun",
                    TamilMeaning = "கருணை / இரக்கம்",
                    EnglishSentence = "Treating homeless animals with compassion is a noble act.",
                    TamilSentence = "வீடற்ற விலங்குகளிடம் கருணையுடன் நடந்துகொள்வது ஒரு சிறந்த செயலாகும்.",
                    Scenario = "Kindness & Empathy (இரக்கம் & அன்பு)"
                },
                new DailyTamilSentence
                {
                    Id = 9,
                    DayNumber = 2,
                    Word = "Fascinating",
                    Phonetic = "/ˈfæsɪneɪtɪŋ/",
                    PartOfSpeech = "Adjective",
                    TamilMeaning = "மனதைக் கவரும் / வியப்பூட்டும்",
                    EnglishSentence = "Space exploration is one of the most fascinating topics in science.",
                    TamilSentence = "விண்வெளி ஆய்வு அறிவியலின் மிகவும் வியப்பூட்டும் தலைப்புகளில் ஒன்றாகும்.",
                    Scenario = "Science & Discovery (அறிவியல் & ஆய்வு)"
                },
                new DailyTamilSentence
                {
                    Id = 10,
                    DayNumber = 2,
                    Word = "Optimistic",
                    Phonetic = "/ˌɒptɪˈmɪstɪk/",
                    PartOfSpeech = "Adjective",
                    TamilMeaning = "நம்பிக்கையான / நன்னோக்கு கொண்ட",
                    EnglishSentence = "Stay optimistic even when things do not go as planned.",
                    TamilSentence = "திட்டமிட்டபடி காரியங்கள் நடக்காவிட்டாலும் நம்பிக்கையோடு இருங்கள்.",
                    Scenario = "Motivation (ஊக்கமளித்தல்)"
                }
            );
        }

    }
}
