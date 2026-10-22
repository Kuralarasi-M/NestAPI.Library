-- ==============================================================================
-- SQL Data Seed Script for LibraryApp "Books" Table
-- Target: PostgreSQL / Supabase
-- Contains curated public domain classics across Tamil, English, French, German, Spanish
-- Matches EF Core Model: Id (IDENTITY), GutendexId, Title, Author, CoverImage, Language, Genre, Summary, Subjects, DownloadCount
-- ==============================================================================

-- 1. TAMIL CLASSICS (தமிழ் இலக்கியங்கள்)
INSERT INTO "Books" ("GutendexId", "Title", "Author", "CoverImage", "Language", "Genre", "Summary", "Subjects", "DownloadCount")
VALUES
(
  29765,
  'திருக்குறள் (Tirukkural)',
  'திருவள்ளுவர் (Thiruvalluvar)',
  'https://www.gutenberg.org/cache/epub/29765/pg29765.cover.medium.jpg',
  'ta',
  'Tamil Literature, Philosophy, Ethics, Poetry',
  'உலகப் பொதுமறை எனப் போற்றப்படும் திருக்குறள் அறம், பொருள், இன்பம் ஆகிய முப்பால்களையும் 1330 குறட்பாக்களில் விளக்கும் உலகப்புகழ் பெற்ற தமிழ் இலக்கிய பொக்கிஷம்.',
  'Tamil literature; Ethics -- Early works to 1800; Tamil poetry; Philosophy',
  45200
),
(
  29987,
  'நாலடியார் (Naladiyar)',
  'சமண முனிவர்கள் (Jain Monks)',
  'https://www.gutenberg.org/cache/epub/29987/pg29987.cover.medium.jpg',
  'ta',
  'Tamil Literature, Moral Poetry, Ancient Works',
  'பதினெண்கீழ்க்கணக்கு நூல்களுள் ஒன்றான நாலடியார் வாழ்வியல் அறநெறிகளையும் நிலையாமையையும் நயம்பட உரைக்கும் சிறந்த நீதி இலக்கிய நூல்.',
  'Tamil poetry -- Early works to 1800; Moral ethics; Tamil philosophy',
  18900
),
(
  32514,
  'நன்னூல் (Nannool - Tamil Grammar)',
  'பவணந்தி முனிவர் (Pavanandhi Munivar)',
  'https://www.gutenberg.org/cache/epub/32514/pg32514.cover.medium.jpg',
  'ta',
  'Tamil Grammar, Linguistics, Educational',
  'தமிழ் மொழியின் எழுத்து மற்றும் சொல் இலக்கணங்களை மிக எளிமையாகவும் தெளிவாகவும் எடுத்துரைக்கும் பிரசித்தி பெற்ற இடைக்கால இலக்கண நூல்.',
  'Tamil language -- Grammar; Linguistics; Classical linguistics',
  12400
),
(
  33418,
  'ஆசாரக்கோவை (Asarakkovai)',
  'பெருவாயின் முள்ளியார் (Peruvayin Mulliyar)',
  'https://www.gutenberg.org/cache/epub/33418/pg33418.cover.medium.jpg',
  'ta',
  'Tamil Literature, Ethics, Daily Conduct',
  'மனித வாழ்வின் அன்றாட ஒழுக்கங்களையும் மனிதநேய நற்பண்புகளையும் அழகாக விவரிக்கும் பதினெண்கீழ்க்கணக்கு நீதி நூல்.',
  'Tamil literature; Conduct of life; Early Tamil ethics',
  15200
),
(
  43442,
  'திரிகடுகம் (Thirikadugam)',
  'நல்லாதனார் (Nallathanar)',
  'https://www.gutenberg.org/cache/epub/43442/pg43442.cover.medium.jpg',
  'ta',
  'Tamil Literature, Ethics, Health & Wisdom',
  'சுக்கு, மிளகு, திப்பிலி உடலுக்கு நலம் தருவது போல், ஒவ்வொரு பாடலிலும் உள்ள மூன்று நீதிகள் உள்ளத்தின் அறியாமையைப் போக்கும் அரிய நூல்.',
  'Tamil literature; Ethical poetry; Wisdom literature',
  14100
),

-- 2. ENGLISH WORLD CLASSICS
(
  1342,
  'Pride and Prejudice',
  'Jane Austen',
  'https://www.gutenberg.org/cache/epub/1342/pg1342.cover.medium.jpg',
  'en',
  'Romance, Classic Fiction, Satire',
  'A classic romantic novel following Elizabeth Bennet as she navigates issues of manners, upbringing, morality, education, and marriage in Regency Britain.',
  'Sisters -- Fiction; Young women -- Fiction; Courtship -- Fiction; England -- Fiction',
  78500
),
(
  84,
  'Frankenstein; Or, The Modern Prometheus',
  'Mary Wollstonecraft Shelley',
  'https://www.gutenberg.org/cache/epub/84/pg84.cover.medium.jpg',
  'en',
  'Gothic, Science Fiction, Horror',
  'The epic gothic tale of Victor Frankenstein, a young scientist who creates a sapient creature in an unorthodox scientific experiment with devastating consequences.',
  'Monsters -- Fiction; Science fiction; Horror tales; Scientists -- Fiction',
  64300
),
(
  11,
  'Alice''s Adventures in Wonderland',
  'Lewis Carroll',
  'https://www.gutenberg.org/cache/epub/11/pg11.cover.medium.jpg',
  'en',
  'Fantasy, Children''s Literature, Adventure',
  'A young girl falls down a rabbit hole into a fantasy realm populated by talking creatures, playing cards, and quirky riddles.',
  'Fantasy fiction; Children''s stories; Imaginary places -- Fiction',
  53200
),
(
  2701,
  'Moby Dick; Or, The Whale',
  'Herman Melville',
  'https://www.gutenberg.org/cache/epub/2701/pg2701.cover.medium.jpg',
  'en',
  'Adventure Fiction, Nautical, Classic',
  'The sailor Ishmael narrates the obsessive quest of Ahab, captain of the whaling ship Pequod, for revenge against Moby Dick, a giant white sperm whale.',
  'Whaling -- Fiction; Sea stories; Psychological fiction; Adventure stories',
  46800
),
(
  145,
  'Middlemarch',
  'George Eliot',
  'https://www.gutenberg.org/cache/epub/145/pg145.cover.medium.jpg',
  'en',
  'Historical Fiction, Victorian, Realism',
  'A panoramic study of provincial life, marriage, idealism, and social reform in 19th-century England through interconnected lives in Middlemarch.',
  'Provincial life -- Fiction; Married life -- Fiction; Great Britain -- History',
  38700
),
(
  98,
  'A Tale of Two Cities',
  'Charles Dickens',
  'https://www.gutenberg.org/cache/epub/98/pg98.cover.medium.jpg',
  'en',
  'Historical Fiction, Classic, Drama',
  'Set in London and Paris before and during the French Revolution, depicting the plight of the French peasantry and brutality toward the former aristocrats.',
  'Historical fiction; France -- History -- Revolution, 1789-1799 -- Fiction; London -- Fiction',
  41200
),

-- 3. FRENCH CLASSICS (Littérature Française)
(
  17989,
  'Les Misérables (Tome I: Fantine)',
  'Victor Hugo',
  'https://www.gutenberg.org/cache/epub/17989/pg17989.cover.medium.jpg',
  'fr',
  'French Literature, Historical Fiction, Drama',
  'Le chef-d''œuvre monumental de Victor Hugo décrivant la rédemption de Jean Valjean et la lutte pour la justice dans la France du XIXe siècle.',
  'Historical fiction; France -- History -- 19th century -- Fiction; Ex-convicts -- Fiction',
  32400
),
(
  4791,
  'Le Comte de Monte-Cristo (Tome I)',
  'Alexandre Dumas',
  'https://www.gutenberg.org/cache/epub/4791/pg4791.cover.medium.jpg',
  'fr',
  'French Literature, Adventure, Revenge',
  'L''histoire légendaire d''Edmond Dantès, trahi et emprisonné au château d''If, qui s''évade pour accomplir une vengeance implacable.',
  'Adventure stories; Revenge -- Fiction; France -- Fiction; Prisoners -- Fiction',
  28900
),
(
  800,
  'Vingt mille lieues sous les mers',
  'Jules Verne',
  'https://www.gutenberg.org/cache/epub/800/pg800.cover.medium.jpg',
  'fr',
  'Science Fiction, French Literature, Adventure',
  'L''expédition extraordinaire du Capitaine Nemo à bord du sous-marin Nautilus à travers les profondeurs mystérieuses des océans.',
  'Submarines -- Fiction; Science fiction, French; Sea stories; Nemo, Captain (Fictitious character)',
  25600
),
(
  13846,
  'Madame Bovary',
  'Gustave Flaubert',
  'https://www.gutenberg.org/cache/epub/13846/pg13846.cover.medium.jpg',
  'fr',
  'French Realism, Drama, Romance',
  'Le portrait réaliste et bouleversant d''Emma Bovary cherchant à échapper à la banalité de la vie provinciale par des liaisons passionnées.',
  'Adultery -- Fiction; Middle class -- France -- Fiction; Suicide victims -- Fiction',
  22100
),

-- 4. GERMAN CLASSICS (Deutsche Literatur)
(
  2229,
  'Faust: Eine Tragödie',
  'Johann Wolfgang von Goethe',
  'https://www.gutenberg.org/cache/epub/2229/pg2229.cover.medium.jpg',
  'de',
  'German Literature, Drama, Philosophy',
  'Goethes Meisterwerk über den Gelehrten Heinrich Faust, der einen folgenschweren Pakt mit dem Teufel Mephistopheles eingeht.',
  'Faust, -approximately 1540 -- Drama; Magicians -- Drama; Devil -- Drama',
  31500
),
(
  5200,
  'Die Verwandlung (The Metamorphosis)',
  'Franz Kafka',
  'https://www.gutenberg.org/cache/epub/5200/pg5200.cover.medium.jpg',
  'de',
  'German Literature, Psychological, Classic',
  'Gregor Samsa erwacht eines Morgens und findet sich in seinem Bett zu einem ungeheuren Ungeziefer verwandelt.',
  'Metamorphosis -- Fiction; Alienation (Social psychology) -- Fiction; Psychological fiction',
  44100
),
(
  2160,
  'Die Leiden des jungen Werthers',
  'Johann Wolfgang von Goethe',
  'https://www.gutenberg.org/cache/epub/2160/pg2160.cover.medium.jpg',
  'de',
  'German Literature, Romance, Epistolary',
  'Der berühmte Briefroman über die unglückliche und leidenschaftliche Liebe des jungen Werther zu Lotte.',
  'Young men -- Germany -- Fiction; Unrequited love -- Fiction; Epistolary fiction',
  23400
),

-- 5. SPANISH CLASSICS (Literatura Española)
(
  2000,
  'Don Quijote de la Mancha',
  'Miguel de Cervantes Saavedra',
  'https://www.gutenberg.org/cache/epub/2000/pg2000.cover.medium.jpg',
  'es',
  'Spanish Literature, Satire, Classic Adventure',
  'La cumbre de la literatura española que narra las divertidas y conmovedoras aventuras del hidalgo Don Quijote y su fiel escudero Sancho Panza.',
  'Knights and knighthood -- Spain -- Fiction; Chivalry -- Fiction; Picaresque literature',
  36700
),
(
  1619,
  'La vida es sueño',
  'Pedro Calderón de la Barca',
  'https://www.gutenberg.org/cache/epub/1619/pg1619.cover.medium.jpg',
  'es',
  'Spanish Theater, Drama, Philosophy',
  'Una de las obras maestras del Siglo de Oro español que reflexiona sobre el libre albedrío, el destino y la naturaleza de la realidad.',
  'Spanish drama (Comedy); Spanish drama -- Classical period, 1500-1700; Philosophy in literature',
  19800
)

ON CONFLICT DO NOTHING;
