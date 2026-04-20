using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;
using System.Security.Claims;

namespace SchoolERP.Net.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
        /// Password hash/salt is generated in SQL stored procedure.
        /// Email/Phone/OTPSecret will be encrypted in the UserService.
        /// </summary>
        [HttpPost("create")]
        public IActionResult CreateUser([FromBody] UserUpsertRequest request)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
                return Unauthorized(new { Success = false, Message = "User is not authenticated." });

            var (result, message) = _userService.CreateUser(request, currentUserId);
            
            if (result > 0)
                return Ok(new { Success = true, Message = message });
            
            return BadRequest(new { Success = false, Message = message });
        }

        /// <summary>
        /// Example of logging in.
        /// Password verification happens in SQL stored procedure.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var (success, message, user) = await _authService.LoginAsync(request.Username, request.Password);

            if (success == 1)
                return Ok(new { Success = true, Message = message, Data = user });

            return Unauthorized(new { Success = false, Message = message });
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("UserId");
            return (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId)) ? userId : 0;
        }
    }
}
