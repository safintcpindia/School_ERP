using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;
using SchoolERP.Net.Services;
using System.Security.Claims;

namespace SchoolERP.Net.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    /// <summary>
    /// Coordinates chronological bounding blocks (Academic Years, Quarters).
    /// Used globally to filter datastreams contextually.
    /// </summary>
    public class SessionApiController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public SessionApiController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        /// <summary>
        /// Constructs a timeline of available master sessions.
        /// </summary>
        [HttpGet("GetAll")]
        public IActionResult GetAll(bool includeDeleted = false)
        {
            var data = _sessionService.GetAllSessions(includeDeleted);
            return Ok(ApiResponse<List<MstSessionViewModel>>.SuccessResponse(data));
        }

        /// <summary>
        /// Reads specific date markers forming a singular tracked calendar state.
        /// </summary>
        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            var data = _sessionService.GetSessionByID(id);
            if (data == null) return NotFound(ApiResponse<MstSessionViewModel>.ErrorResponse("Time phase interval not found"));
            return Ok(ApiResponse<MstSessionViewModel>.SuccessResponse(data));
        }

        /// <summary>
        /// Declares and modifies rigid boundaries defining a new system session block.
        /// </summary>
        [HttpPost("Upsert")]
        public IActionResult Upsert([FromBody] MstSessionUpsertRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            int userId = GetCurrentUserId();
            var (success, message) = _sessionService.UpsertSession(request, userId);
            return Ok(new { success, message });
        }

        /// <summary>
        /// Structurally nulls an unused operating interval block.
        /// </summary>
        [HttpPost("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            int userId = GetCurrentUserId();
            var (success, message) = _sessionService.DeleteSession(id, userId);
            return Ok(new { success, message });
        }

        /// <summary>
        /// Visually deprioritizes historical terms from active menu selection streams.
        /// </summary>
        [HttpPost("ToggleStatus")]
        public IActionResult ToggleStatus(int id, bool isActive)
        {
            int userId = GetCurrentUserId();
            var (success, message) = _sessionService.ToggleSessionStatus(id, isActive, userId);
            return Ok(new { success, message });
        }

        /// <summary>
        /// Resolves cryptographically bound credentials parsing the requesting context.
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1; 
        }
    }
}
