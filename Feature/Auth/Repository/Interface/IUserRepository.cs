using LibraryApp.Feature.Auth.Commands;
using LibraryApp.Models;

namespace LibraryApp.Feature.Auth.Repository.Interface
{
    public interface IUserRepository
    {
        Task<bool> UserAlreadyExist(string email);
        Task<bool> UserAddAsync(User user);
        Task<User> UserVerify(UserLoginReq user);
        Task<User?> GetUserByEmailAsync(string email);
    }
}
