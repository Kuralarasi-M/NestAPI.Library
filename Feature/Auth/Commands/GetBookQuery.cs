using MediatR;

namespace LibraryApp.Feature.Auth.Commands
{
    public class GetBookQuery : IRequest<GetBookResponseDTO>
    {

        public GetBookQuery(int bookId)
        {
            BookId = bookId;
        }

        public int BookId { get; }
    }
    public class GetBookResponseDTO
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public List<AuthorDTO> Authors { get; set; } = [];

        public List<string> Summaries { get; set; } = [];

        public List<string> Subjects { get; set; } = [];

        public List<string> Languages { get; set; } = [];
        public string BookText { get; set; } = string.Empty;

        public Dictionary<string, string> Formats { get; set; } = [];

        public int Download_Count { get; set; }
    }

    public class AuthorDTO
    {
        public string Name { get; set; } = string.Empty;

        public int? Birth_Year { get; set; }

        public int? Death_Year { get; set; }
    }
}