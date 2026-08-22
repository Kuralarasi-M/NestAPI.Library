using MediatR;
using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Feature.Auth.Commands
{
    public class AllBooksQuery:IRequest<IEnumerable<BooksResponse>>
    {

    }
    public class BooksResponse
    {
        public long Id { get; set; }

        public int GutendexId { get; set; }

        
        public string Title { get; set; } = string.Empty;


        public string Author { get; set; } = string.Empty;

        public string CoverImage { get; set; } = string.Empty;

     
        public string Language { get; set; } = string.Empty;

        // Stores multiple genres/categories separated by commas
        public string Genre { get; set; } = string.Empty;

        // Book description from Gutendex
        public string Summary { get; set; } = string.Empty;

        // Stores multiple subjects separated by commas
        public string Subjects { get; set; } = string.Empty;

        // Number of downloads from Gutendex
        public int DownloadCount { get; set; }
    }
}
