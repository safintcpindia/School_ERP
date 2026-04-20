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
    /// Exposes core dictionary mappings for generic system types (e.g. Employee vs Student).
    /// </summary>
    public class UserTypeApiController : ControllerBase
    {
        private readonly IUserManagementService _userMgmtService;

        public UserTypeApiController(IUserManagementService userMgmtService)
        {
            _userMgmtService = userMgmtService;
        }

        /// <summary>
        /// Reads the array of User Type metadata structures.
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var data = _userMgmtService.GetAllUserTypes();
            return Ok(ApiResponse<List<MstUserTypeViewModel>>.SuccessResponse(data));
        }

        /// <summary>
        /// Fetches a distinct configuration profile for a single user type ID.
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
        /// Enters new mapping categories into the data layer natively.
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
        /// Strips a mapping from active views organically without dropping foreign keys.
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
