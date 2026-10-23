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
            
            // 1. Send FCM push notification to all Admins
            try
            {
                await _notificationService.NotifyAdminsAsync(
                    "New User Registered",
                    $"{user.Name} ({user.Email}) registered successfully."
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FCM Error on Register]: {ex.Message}");
            }

            // 2. Save in-app notification in DB for all Admin users so they see it in their notification feed
            var adminUserIds = await _dbContext.Users
                .Where(u => u.Role == "Admin")
                .Select(u => u.Id)
                .ToListAsync(cancellationToken);

            foreach (var adminId in adminUserIds)
            {
                await _dbContext.Notifications.AddAsync(new Notification
                {
                    UserId = adminId,
                    Title = "New User Registered",
                    Message = $"{user.Name} ({user.Email}) has registered successfully.",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                }, cancellationToken);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
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
