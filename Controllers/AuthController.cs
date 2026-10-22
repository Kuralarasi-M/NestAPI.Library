using LibraryApp.Feature.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LibraryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator mediator;
        public AuthController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> UserRegister([FromBody]UserRegisterCommand command)
        {
            if (command == null)
                return BadRequest("Invalid request");

            var result = await mediator.Send(command);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> UserLogin([FromBody]UserLoginReq user)
        {
            var result = await mediator.Send(user);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("FCMToken")]
        public async Task<IActionResult> FcmSave([FromBody] TokenRequest token)
        {
            var result = await mediator.Send(token);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("Notification")]
        public async Task<IActionResult> NotificationAsync()
        {
            var result = await mediator.Send(new NotficationReq());
            return Ok(result);
        }
    }
}
