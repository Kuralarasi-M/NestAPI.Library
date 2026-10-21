using LibraryApp.Models;

namespace LibraryApp.Feature.Auth.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
