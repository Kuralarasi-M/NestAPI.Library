using LibraryApp.Feature.Auth.Commands;
using LibraryApp.Feature.Auth.Repository.Interface;
using LibraryApp.Feature.Auth.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LibraryApp.Feature.Auth.Handler
{
    public class UserLoginHandler : IRequestHandler<UserLoginReq, UserLoginResponse>
    {
        private readonly IUserRepository _repository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<UserLoginHandler> _logger;

        public UserLoginHandler(
            IUserRepository repository,
            IJwtTokenGenerator jwtTokenGenerator,
            IServiceScopeFactory scopeFactory,
            ILogger<UserLoginHandler> logger)
        {
            _repository = repository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }
        public async Task<UserLoginResponse> Handle(UserLoginReq request, CancellationToken cancellationToken)
        {
            UserLoginResponse res = new UserLoginResponse();

            if (string.IsNullOrWhiteSpace(request?.Email) || string.IsNullOrWhiteSpace(request?.Password))
            {
                res.Message = "Email and password are required.";
                res.IsSucess = false;
                res.Token = "";
                return res;
            }

            var user = await _repository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                res.Message = "User Not Exist!. Please register First";
                res.IsSucess = false;
                res.Token = "";
                return res;
            }

            if (user.PasswordHash != request.Password)
            {
                res.Message = "User Email or Password is Wrong!.";
                res.IsSucess = false;
                res.Token = "";
                return res;
            }

            var token = _jwtTokenGenerator.GenerateToken(user);

            res.Message = "User Log-in Successfully!.";
            res.IsSucess = true;
            res.Token = token;
            res.Role = user.Role;
            res.userId = user.Id;

            // Send FCM notification to admins when a user logs in (excluding admin's own logins)
            // Use a separate scope so DbContext is not disposed after the HTTP request finishes
            if (!string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                var userName = user.Name;
                var userEmail = user.Email;
                var userId = user.Id;

                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var notifService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                        var title = "User Login Alert";
                        var body = $"User '{userName}' ({userEmail}) logged in at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
                        await notifService.NotifyAdminsAsync(title, body);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send login notification to admins for user {UserId}", userId);
                    }
                });
            }

            return res;
        }
    }
}

