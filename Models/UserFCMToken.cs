namespace LibraryApp.Models
{
    public class UserFcmToken
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public string FcmToken { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public User User { get; set; } = null!;
    }
}
