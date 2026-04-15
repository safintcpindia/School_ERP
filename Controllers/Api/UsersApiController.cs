using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;

namespace SchoolERP.Net.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    /// <summary>
    /// Alternate simplified routing segment handling Create/Login constructs directly.
    /// Functions similarly to UserApiController but explicitly partitions auth.
    /// </summary>
    public class UsersApiController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public UsersApiController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        /// <summary>
        /// Example of creating a user. 
        /// Password will be hashed in the UserService before being sent to SQL.
        /// Email/Phone/OTPSecret will be encrypted in the UserService.
        /// </summary>
        [HttpPost("create")]
        public IActionResult CreateUser([FromBody] UserUpsertRequest request)
        {
            // In a real app, get current UserID from JWT
            int currentUserId = 1; 

            var (result, message) = _userService.CreateUser(request, currentUserId);
            
            if (result > 0)
                return Ok(new { Success = true, Message = message });
            
            return BadRequest(new { Success = false, Message = message });
        }

        /// <summary>
        /// Example of logging in.
        /// Password verification happens in AuthService (.NET), not in SQL.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var (success, message, user) = await _authService.LoginAsync(request.Username, request.Password);

            if (success == 1)
                return Ok(new { Success = true, Message = message, Data = user });

            return Unauthorized(new { Success = false, Message = message });
        }
    }
}
