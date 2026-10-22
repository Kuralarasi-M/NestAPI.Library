using MediatR;
namespace LibraryApp.Feature.Auth.Commands
{
    public class UserRegisterCommand : IRequest<UserRegisterResponse>
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class UserRegisterResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Role {  get; set; } = string.Empty;
        public Guid? userId { get; set; }
    }
}
