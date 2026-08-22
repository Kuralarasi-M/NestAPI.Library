using LibraryApp.Feature.Auth.Commands;
using LibraryApp.Feature.Auth.Repository.Interface;
using LibraryApp.Models;
using MediatR;

namespace LibraryApp.Feature.Auth.Handler
{
    public class UserRegisterHandler : IRequestHandler<UserRegisterCommand, UserRegisterResponse>
    {
        private readonly IUserRepository repository;
        public UserRegisterHandler(IUserRepository repository)
        {
            this.repository = repository;
        }
        public async Task<UserRegisterResponse> Handle(UserRegisterCommand request,CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return new UserRegisterResponse
                {
                    Success= false,
                    Message  = "Email is required"
                };
            }
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return new UserRegisterResponse
                {
                    Success = false,
                    Message = "Email is required"
                };
            }
            var exists = await repository.UserAlreadyExist(request.Email);
            if (exists)
            {
                return new UserRegisterResponse
                {
                    Success = false,
                    Message = "User is Already Exist!."
                };
            }
            var user = new User
            {
                Name = request.Email,
                Email = request.Email,
                PasswordHash = request.Password
            };
            await repository.UserAddAsync(user);
            return new UserRegisterResponse
            {
                Success = true,
                Message = "User Created Successfully!.",
                Role = user.Role,
                userId=user.Id
            };
        }
    }
    
}
