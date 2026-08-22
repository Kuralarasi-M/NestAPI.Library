using LibraryApp.Data;
using LibraryApp.Feature.Auth.Commands;
using LibraryApp.Feature.Auth.Repository.Interface;
using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Feature.Auth.Repository.Implementation
{

    public class UserRepository : IUserRepository
    {
        private readonly AppDBContext context;
        public UserRepository(AppDBContext context)
        {
            this.context = context;
        }

        public async Task<bool> UserAddAsync(User user)
        {
            if (user == null)
            {
                return false;
            }
            context.AddAsync(user);
            var result = await context.SaveChangesAsync();
            return result> 0;
        }

        public async Task<bool> UserAlreadyExist(string email)
        {
            return await context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }
        public async Task<User> UserVerify(UserLoginReq user)
        {
            var response = await context.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.PasswordHash==user.Password);
            return response;
        }

    }
}
