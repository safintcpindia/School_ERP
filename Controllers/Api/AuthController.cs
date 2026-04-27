using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;
using SchoolERP.Net.Services;

namespace SchoolERP.Net.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    /// <summary>
    /// This controller provides the technical endpoints for verifying user credentials and creating secure access tokens.
    /// </summary>
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Verifies the username and password against the database and provides a secure access token if they are correct.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Failsafe parameter injection block
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest(ApiResponse<UserSessionModel>.ErrorResponse("Invalid request parameters"));

            // Funnels strings down into the auth service for database cross-checking
            var result = await _authService.LoginAsync(request.Username, request.Password);

            // '1' traditionally maps to an exact DB match
            if (result.Success == 1)
            {
                return Ok(ApiResponse<UserSessionModel>.SuccessResponse(result.User, result.Message));
            }
            
            return Unauthorized(ApiResponse<UserSessionModel>.ErrorResponse(result.Message));
        }
    }
}
