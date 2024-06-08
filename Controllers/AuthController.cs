using Microsoft.AspNetCore.Mvc;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] User user)
        {
            var authenticatedUser = await _userService.Authenticate(user.Username, user.PasswordHash);
            if (authenticatedUser == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(authenticatedUser);
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] User user)
        {
            var registeredUser = await _userService.Register(user, user.PasswordHash);
            if (registeredUser == null)
                return BadRequest(new { message = "User could not be registered" });

            return Ok(registeredUser);
        }
    }
}