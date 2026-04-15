using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;
using SchoolERP.Net.Services;
using System.Collections.Generic;

namespace SchoolERP.Net.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    /// <summary>
    /// Processes granular security matrices routing mapping arrays to access tiers.
    /// </summary>
    public class RoleApiController : ControllerBase
    {
        private readonly IUserManagementService _userMgmtService;

        public RoleApiController(IUserManagementService userMgmtService)
        {
            _userMgmtService = userMgmtService;
        }

        /// <summary>
        /// Reads all global roles inside the current environment string.
        /// </summary>
        [HttpGet]
        public IActionResult GetAllRoles()
        {
            var roles = _userMgmtService.GetAllRoles();
            return Ok(ApiResponse<List<MstRoleViewModel>>.SuccessResponse(roles));
        }

        /// <summary>
        /// Connects logic targeting a singular role's property sets.
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetRoleById(int id)
        {
            var role = _userMgmtService.GetRoleByID(id);
            if (role == null) return NotFound(ApiResponse<MstRoleViewModel>.ErrorResponse("Role not found from specified identifier"));
            return Ok(ApiResponse<MstRoleViewModel>.SuccessResponse(role));
        }

        /// <summary>
        /// Crafts or modifies role definitions structurally.
        /// </summary>
        [HttpPost("save")]
        public IActionResult Save([FromBody] MstRoleUpsertRequest request)
        {
            int currentUserId = 1;
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var result = _userMgmtService.UpsertRole(request, currentUserId, ipAddress);
            
            if (result.success)
                return Ok(ApiResponse<int>.SuccessResponse(result.roleId, result.message));
            
            return BadRequest(ApiResponse<int>.ErrorResponse(result.message));
        }

        /// <summary>
        /// Drops a role mapping natively across attached profiles logically.
        /// </summary>
        [HttpPost("toggle-status")]
        public IActionResult ToggleStatus([FromQuery] int roleId, [FromQuery] bool isActive)
        {
            int currentUserId = 1;
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var result = _userMgmtService.ToggleRoleStatus(roleId, isActive, currentUserId, ipAddress);
            
            if (result.success)
                return Ok(ApiResponse<bool>.SuccessResponse(true, result.message));
            
            return BadRequest(ApiResponse<bool>.ErrorResponse(result.message));
        }

        /// <summary>
        /// Resolves the comprehensive hierarchical array (Menus and View/Edit checks) applied to this Role.
        /// </summary>
        [HttpGet("{id}/permissions")]
        public IActionResult GetPermissions(int id)
        {
            var matrix = _userMgmtService.GetPermissionsMatrix(id);
            return Ok(ApiResponse<List<RoleMenuPermissionViewModel>>.SuccessResponse(matrix));
        }

        /// <summary>
        /// Purges and resyncs the active checklist items globally defining what a Role executes.
        /// </summary>
        [HttpPost("save-permissions")]
        public IActionResult SavePermissions([FromBody] MstRolePermissionSaveRequest request)
        {
            int currentUserId = 1;
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var result = _userMgmtService.SaveRolePermissions(request, currentUserId, ipAddress);
            
            if (result.success)
                return Ok(ApiResponse<bool>.SuccessResponse(true, result.message));
            
            return BadRequest(ApiResponse<bool>.ErrorResponse(result.message));
        }
    }
}
