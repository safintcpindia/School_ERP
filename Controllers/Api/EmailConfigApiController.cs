using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;
using SchoolERP.Net.Services;
using System.Security.Claims;

namespace SchoolERP.Net.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    /// <summary>
    /// This class handles HTTP routing and API requests for EmailConfigApiController.
    /// </summary>
    public class EmailConfigApiController : ControllerBase
    {
        private readonly IEmailConfigService _emailConfigService;

        public EmailConfigApiController(IEmailConfigService emailConfigService)
        {
            _emailConfigService = emailConfigService;
        }

        /// <summary>
        /// Extracts the active SMTP credentials safely stripped for administrative review.
        /// </summary>
        [HttpGet("Get")]
        public IActionResult Get()
        {
            var data = _emailConfigService.GetEmailConfig();
            return Ok(ApiResponse<MstEmailConfigViewModel>.SuccessResponse(data));
        }

        /// <summary>
        /// Commits new mail server parameters (host, port, ssl settings) and stores audit logs.
        /// </summary>
        [HttpPost("Upsert")]
        public IActionResult Upsert([FromBody] MstEmailConfigUpsertRequest request)
        {
            // Abort early if the port constraints or payload strings fail model bounds
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            int userId = GetCurrentUserId();
            var (success, message) = _emailConfigService.UpsertEmailConfig(request, userId);
            
            if (success) return Ok(ApiResponse<dynamic>.SuccessResponse(new { success }, message));
            return BadRequest(ApiResponse<dynamic>.ErrorResponse(message));
        }

        /// <summary>
        /// Reads the JWT or encrypted cookie ClaimsPrincipal to identify the administrator acting context.
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            // Defaulting to user '1' if token claims are bypassed or missing
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }
    }
}
