using LibraryApp.Feature.Auth.Commands;
using LibraryApp.Feature.Auth.Repository.Interface;
using MediatR;

namespace LibraryApp.Feature.Auth.Handler
{
    public class UserLoginHandler : IRequestHandler<UserLoginReq, UserLoginResponse>
    {

        private readonly IUserRepository repository;
        public UserLoginHandler(IUserRepository repository)
        {
            this.repository = repository;
        }
        public async Task<UserLoginResponse> Handle(UserLoginReq request, CancellationToken cancellationToken)
        {
            UserLoginResponse res = new UserLoginResponse();
            var response = await repository.UserAlreadyExist(request.Email);
            if (response!=true)
            {
                res.Message = "User Not Exist!. Please register First";
                res.IsSucess= false;
                res.Token = "";
                return res;
            }
            var user = await repository.UserVerify(request);
            if (user!=null)
            {
                res.Message = "User Log-in Successfully!.";
                res.IsSucess = true;
                res.Token = "UserToken";
                res.Role= user.Role;
                res.userId = user.Id;
                return res;
            }
            res.Message = "User Email or Password is Wrong!.";
            res.IsSucess = false;
            res.Token = "";
            return res;
        }
        
    }
}
