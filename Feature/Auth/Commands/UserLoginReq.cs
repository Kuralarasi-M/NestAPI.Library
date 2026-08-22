using MediatR;

namespace LibraryApp.Feature.Auth.Commands
{
    public class UserLoginReq:IRequest<UserLoginResponse>
    {
        public string Email {  get; set; }
        public string Password { get; set; }
    }
    public class UserLoginResponse
    {
        public bool IsSucess { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
        public string Role { get; set; }

        public int userId { get; set; }
    }

}
