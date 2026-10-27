using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibraryApp.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyTamilSentences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyTamilSentences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Word = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Phonetic = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PartOfSpeech = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TamilMeaning = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EnglishSentence = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TamilSentence = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Scenario = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DayNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTamilSentences", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "DailyTamilSentences",
                columns: new[] { "Id", "DayNumber", "EnglishSentence", "PartOfSpeech", "Phonetic", "Scenario", "TamilMeaning", "TamilSentence", "Word" },
                values: new object[,]
                {
                    { 1, 1, "Success in any competitive exam requires patience and perseverance.", "Noun", "/ˌpɜːsɪˈvɪərəns/", "Career & Exam Preparation (வேலை & கல்வி சூழல்)", "விடாமுயற்சி", "போட்டித் தேர்வுகளில் வெற்றி பெற பொறுமையும் விடாமுயற்சியும் அவசியம்.", "Perseverance" },
                    { 2, 1, "The students were very enthusiastic about the new science project.", "Adjective", "/ɪnˌθjuːziˈæstɪk/", "School & Learning (பள்ளி மற்றும் கற்றல் சூழல்)", "ஆர்வமிக்க / உற்சாகமான", "புதிய அறிவியல் திட்டத்தில் மாணவர்கள் மிகவும் ஆர்வமாகவும் உற்சாகமாகவும் இருந்தனர்.", "Enthusiastic" },
                    { 3, 1, "The ancient Tanjore temple has a magnificent architecture.", "Adjective", "/mæɡˈnɪfɪsnt/", "Travel & Culture (சுற்றுலா & கலாச்சாரம்)", "அற்புதமான / கம்பீரமான", "பழங்கால தஞ்சைப் பெரிய கோவில் அற்புதமான கட்டடக்கலை கொண்டது.", "Magnificent" },
                    { 4, 1, "He expressed his sincere gratitude to everyone who helped him.", "Noun", "/ˈɡrætɪtjuːd/", "Social Courtesy (சமூக நற்பண்பு)", "நன்றி உணர்வு", "தனக்கு உதவிய அனைவருக்கும் அவர் தனது உளமார்ந்த நன்றி உணர்வைத் தெரிவித்தார்.", "Gratitude" },
                    { 5, 1, "She remained resilient despite facing numerous unexpected difficulties.", "Adjective", "/rɪˈzɪliənt/", "Overcoming Obstacles (சவால்களை எதிர்கொள்ளுதல்)", "மீண்டுவரும் தன்மை கொண்ட / தளராத", "பல எதிர்பாராத இன்னல்களை சந்தித்த போதிலும், அவள் தளராமல் மீண்டு வந்தாள்.", "Resilient" },
                    { 6, 2, "Children learn quickly because of their natural curiosity.", "Noun", "/ˌkjʊəriˈɒsəti/", "Everyday Life (அன்றாட வாழ்க்கை)", "அறியும் ஆர்வம் / புதுமை நாட்டம்", "இயற்கையான அறியும் ஆர்வத்தின் காரணமாக குழந்தைகள் விரைவாகக் கற்றுக்கொள்கிறார்கள்.", "Curiosity" },
                    { 7, 2, "Being punctual to morning meetings shows your professional dedication.", "Adjective", "/ˈpʌŋktʃuəl/", "Workplace & Office (அலுவலகச் சூழல்)", "நேரந்தவறாமை", "காலை கூட்டங்களுக்கு நேரந்தவறாமல் வருவது உங்கள் தொழில் அர்ப்பணிப்பைக் காட்டுகிறது.", "Punctual" },
                    { 8, 2, "Treating homeless animals with compassion is a noble act.", "Noun", "/kəmˈpæʃn/", "Kindness & Empathy (இரக்கம் & அன்பு)", "கருணை / இரக்கம்", "வீடற்ற விலங்குகளிடம் கருணையுடன் நடந்துகொள்வது ஒரு சிறந்த செயலாகும்.", "Compassion" },
                    { 9, 2, "Space exploration is one of the most fascinating topics in science.", "Adjective", "/ˈfæsɪneɪtɪŋ/", "Science & Discovery (அறிவியல் & ஆய்வு)", "மனதைக் கவரும் / வியப்பூட்டும்", "விண்வெளி ஆய்வு அறிவியலின் மிகவும் வியப்பூட்டும் தலைப்புகளில் ஒன்றாகும்.", "Fascinating" },
                    { 10, 2, "Stay optimistic even when things do not go as planned.", "Adjective", "/ˌɒptɪˈmɪstɪk/", "Motivation (ஊக்கமளித்தல்)", "நம்பிக்கையான / நன்னோக்கு கொண்ட", "திட்டமிட்டபடி காரியங்கள் நடக்காவிட்டாலும் நம்பிக்கையோடு இருங்கள்.", "Optimistic" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyTamilSentences");
        }
    }
}
