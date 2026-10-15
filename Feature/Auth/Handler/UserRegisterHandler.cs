using Google;
using LibraryApp.Data;
using LibraryApp.Feature.Auth.Commands;
using LibraryApp.Feature.Auth.Repository.Interface;
using LibraryApp.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Feature.Auth.Handler
{
    public class UserRegisterHandler : IRequestHandler<UserRegisterCommand, UserRegisterResponse>
    {
        private readonly IUserRepository repository;
        private readonly INotificationService _notificationService;
        private readonly AppDBContext _dbContext;

        public UserRegisterHandler(IUserRepository repository,INotificationService service,AppDBContext context)
        {
            this.repository = repository;
            this._notificationService = service;
            this._dbContext = context;
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
                Name = request.UserName,
                Email = request.Email,
                PasswordHash = request.Password
            };
            await repository.UserAddAsync(user);
            
            await _notificationService.NotifyAdminsAsync(
                "New User Registered",
                $"{user.Name} registered successfully."
            );
            var notifi = new Notification
            {
                Title = $"New User Registered",
                Message= $"{user.Name} has registered successfully.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                UserId= user.Id

            };
            await _dbContext.Notifications.AddAsync(notifi);
            await _dbContext.SaveChangesAsync();
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
