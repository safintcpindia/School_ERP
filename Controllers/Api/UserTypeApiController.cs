using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;
using SchoolERP.Net.Services;
using System.Collections.Generic;
using System.Security.Claims;

namespace SchoolERP.Net.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    /// <summary>
    /// This controller provides the technical endpoints for managing different categories of users (like 'Employee' or 'Student') through the API.
    /// </summary>
    public class UserTypeApiController : ControllerBase
    {
        private readonly IUserManagementService _userMgmtService;

        public UserTypeApiController(IUserManagementService userMgmtService)
        {
            _userMgmtService = userMgmtService;
        }

        /// <summary>
        /// Gets the full list of all defined user categories from the system.
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var data = _userMgmtService.GetAllUserTypes();
            return Ok(ApiResponse<List<MstUserTypeViewModel>>.SuccessResponse(data));
        }

        /// <summary>
        /// Gets the details of one specific user category using its unique ID number.
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var data = _userMgmtService.GetUserTypeByID(id);
            if (data == null)
                return NotFound(ApiResponse<MstUserTypeViewModel>.ErrorResponse("User type not found"));
            
            return Ok(ApiResponse<MstUserTypeViewModel>.SuccessResponse(data));
        }

        /// <summary>
        /// Saves a new user category or updates an existing one with the name you provided.
        /// </summary>
        [HttpPost("save")]
        public IActionResult Save([FromBody] MstUserTypeUpsertRequest request)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated."));
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";

            var result = _userMgmtService.UpsertUserType(request, currentUserId, ipAddress);
            if (result.success)
                return Ok(ApiResponse<bool>.SuccessResponse(true, result.message));
            
            return BadRequest(ApiResponse<bool>.ErrorResponse(result.message));
        }

        /// <summary>
        /// Turns a user category on or off.
        /// </summary>
        [HttpPost("toggle-status")]
        public IActionResult ToggleStatus([FromQuery] int typeId, [FromQuery] bool isActive)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated."));
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";

            var result = _userMgmtService.ToggleUserTypeStatus(typeId, isActive, currentUserId, ipAddress);
            if (result.success)
                return Ok(ApiResponse<bool>.SuccessResponse(true, result.message));

            return BadRequest(ApiResponse<bool>.ErrorResponse(result.message));
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("UserId");
            return (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId)) ? userId : 0;
        }
    }
}
