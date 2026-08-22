namespace LibraryApp.Data
{
    public class GetBookDTO
    {
        public int GutendexId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public string CoverImage { get; set; } = string.Empty;

        public string Language { get; set; } = string.Empty;

        public List<string> Subjects { get; set; } = [];

        public string ReadUrl { get; set; } = string.Empty;

        public string DownloadUrl { get; set; } = string.Empty;
    }
}
