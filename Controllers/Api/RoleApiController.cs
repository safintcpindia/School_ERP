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
    /// This controller provides the technical endpoints for managing user roles and their permissions through the API.
    /// </summary>
    public class RoleApiController : ControllerBase
    {
        private readonly IUserManagementService _userMgmtService;

        public RoleApiController(IUserManagementService userMgmtService)
        {
            _userMgmtService = userMgmtService;
        }

        /// <summary>
        /// Gets the full list of all defined user roles from the system.
        /// </summary>
        [HttpGet]
        public IActionResult GetAllRoles()
        {
            var roles = _userMgmtService.GetAllRoles();
            return Ok(ApiResponse<List<MstRoleViewModel>>.SuccessResponse(roles));
        }

        /// <summary>
        /// Gets the details of one specific role using its unique ID number.
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetRoleById(int id)
        {
            var role = _userMgmtService.GetRoleByID(id);
            if (role == null) return NotFound(ApiResponse<MstRoleViewModel>.ErrorResponse("Role not found from specified identifier"));
            return Ok(ApiResponse<MstRoleViewModel>.SuccessResponse(role));
        }

        /// <summary>
        /// Saves a new user role or updates an existing one with the details you provided.
        /// </summary>
        [HttpPost("save")]
        public IActionResult Save([FromBody] MstRoleUpsertRequest request)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
                return Unauthorized(ApiResponse<int>.ErrorResponse("User is not authenticated."));
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var result = _userMgmtService.UpsertRole(request, currentUserId, ipAddress);
            
            if (result.success)
                return Ok(ApiResponse<int>.SuccessResponse(result.roleId, result.message));
            
            return BadRequest(ApiResponse<int>.ErrorResponse(result.message));
        }

        /// <summary>
        /// Turns a role's active status on or off.
        /// </summary>
        [HttpPost("toggle-status")]
        public IActionResult ToggleStatus([FromQuery] int roleId, [FromQuery] bool isActive)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated."));
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var result = _userMgmtService.ToggleRoleStatus(roleId, isActive, currentUserId, ipAddress);
            
            if (result.success)
                return Ok(ApiResponse<bool>.SuccessResponse(true, result.message));
            
            return BadRequest(ApiResponse<bool>.ErrorResponse(result.message));
        }

        /// <summary>
        /// Gets the list of allowed actions (like viewing or editing specific menus) for a chosen role.
        /// </summary>
        [HttpGet("{id}/permissions")]
        public IActionResult GetPermissions(int id)
        {
            var matrix = _userMgmtService.GetPermissionsMatrix(id);
            return Ok(ApiResponse<List<RoleMenuPermissionViewModel>>.SuccessResponse(matrix));
        }

        /// <summary>
        /// Saves the chosen list of allowed actions for a role.
        /// </summary>
        [HttpPost("save-permissions")]
        public IActionResult SavePermissions([FromBody] MstRolePermissionSaveRequest request)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated."));
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var result = _userMgmtService.SaveRolePermissions(request, currentUserId, ipAddress);
            
            if (result.success)
                return Ok(ApiResponse<bool>.SuccessResponse(true, result.message));
            
            return BadRequest(ApiResponse<bool>.ErrorResponse(result.message));
        }

        /// <summary>
        /// Permanently removes a role from the system's records.
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated."));

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var result = _userMgmtService.DeleteRole(id, currentUserId, ipAddress);
            
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
