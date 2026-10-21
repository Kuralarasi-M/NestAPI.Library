using LibraryApp.Feature.Auth.Commands;
using LibraryApp.Feature.Auth.Repository.Interface;
using LibraryApp.Feature.Auth.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryApp.Feature.Auth.Handler
{
    public class UserLoginHandler : IRequestHandler<UserLoginReq, UserLoginResponse>
    {
        private readonly IUserRepository _repository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly INotificationService _notificationService;
        private readonly ILogger<UserLoginHandler> _logger;

        public UserLoginHandler(
            IUserRepository repository,
            IJwtTokenGenerator jwtTokenGenerator,
            INotificationService notificationService,
            ILogger<UserLoginHandler> logger)
        {
            _repository = repository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<UserLoginResponse> Handle(UserLoginReq request, CancellationToken cancellationToken)
        {
            UserLoginResponse res = new UserLoginResponse();
            var response = await _repository.UserAlreadyExist(request.Email);
            if (response != true)
            {
                res.Message = "User Not Exist!. Please register First";
                res.IsSucess = false;
                res.Token = "";
                return res;
            }

            var user = await _repository.UserVerify(request);
            if (user != null)
            {
                var token = _jwtTokenGenerator.GenerateToken(user);

                res.Message = "User Log-in Successfully!.";
                res.IsSucess = true;
                res.Token = token;
                res.Role = user.Role;
                res.userId = user.Id;

                // Send FCM notification to admins when a user logs in (excluding admin's own logins)
                if (!string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            var title = "User Login Alert";
                            var body = $"User '{user.Name}' ({user.Email}) logged in at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
                            await _notificationService.NotifyAdminsAsync(title, body);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send login notification to admins for user {UserId}", user.Id);
                        }
                    }, cancellationToken);
                }

                return res;
            }

            res.Message = "User Email or Password is Wrong!.";
            res.IsSucess = false;
            res.Token = "";
            return res;
        }
    }
}

