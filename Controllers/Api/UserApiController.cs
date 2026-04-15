using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;
using SchoolERP.Net.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers.Api
{
    [Route("api/user")]
    [ApiController]
    /// <summary>
    /// Centralized integration API for CRUD operations against the tbl_mst_users dataset.
    /// Used by frontend Javascript bundles to avoid page reloads.
    /// </summary>
    public class UserApiController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserApiController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Renders the array of active administrators and staff for datatable hydration.
        /// </summary>
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            return Ok(ApiResponse<List<UserViewModel>>.SuccessResponse(users));
        }

        /// <summary>
        /// Identifies an individual user object for profile edits.
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _userService.GetUserById(id);
            if (user == null) return NotFound(ApiResponse<UserViewModel>.ErrorResponse("User not found"));
            return Ok(ApiResponse<UserViewModel>.SuccessResponse(user));
        }

        /// <summary>
        /// Reads the specific role matrix array bound to the selected user's profile.
        /// </summary>
        [HttpGet("{id}/roles")]
        public IActionResult GetUserRoleIds(int id)
        {
            var roleIds = _userService.GetUserRoleIds(id);
            return Ok(ApiResponse<List<int>>.SuccessResponse(roleIds));
        }

        /// <summary>
        /// Lookup endpoints for constructing static HTML dropdown `<select>` modules dynamically.
        /// </summary>
        [HttpGet("roles-dropdown")]
        public IActionResult GetRoles()
        {
            var roles = _userService.GetRoles();
            return Ok(ApiResponse<List<RoleViewModel>>.SuccessResponse(roles));
        }

        /// <summary>
        /// Renders available role boundary types (Admin, Teacher, Staff).
        /// </summary>
        [HttpGet("types-dropdown")]
        public IActionResult GetUserTypes()
        {
            var types = _userService.GetUserTypes();
            return Ok(ApiResponse<List<MstUserTypeViewModel>>.SuccessResponse(types));
        }

        /// <summary>
        /// Interprets a User create/edit modal submission from frontend API clients.
        /// Dispatches data to SQL Server encryption routines.
        /// </summary>
        [HttpPost("save")]
        public IActionResult Save([FromBody] UserUpsertRequest request)
        {
            // TODO: Ensure JWT Claim stripping validates identity to prevent spoofing
            int actingUser = 1; 
            (int result, string message) response;

            // Route dynamically based on Primary Key state
            if (request.UserID == 0)
                response = _userService.CreateUser(request, actingUser);
            else
                response = _userService.UpdateUser(request, actingUser);

            if (response.result > 0)
                return Ok(ApiResponse<bool>.SuccessResponse(true, response.message));
            
            return BadRequest(ApiResponse<bool>.ErrorResponse(response.message));
        }

        /// <summary>
        /// Administratively blocks or permits logins.
        /// </summary>
        [HttpPost("toggle-status")]
        public IActionResult ToggleStatus([FromQuery] int userId, [FromQuery] bool isActive)
        {
            int actingUser = 1;
            var response = _userService.ToggleUserStatus(userId, isActive, actingUser);
            if (response.Result > 0)
                return Ok(ApiResponse<bool>.SuccessResponse(true, response.Message));
            
            return BadRequest(ApiResponse<bool>.ErrorResponse(response.Message));
        }

        /// <summary>
        /// Purges security lockouts generated from repeated failing password attempts.
        /// </summary>
        [HttpPost("unlock/{id}")]
        public IActionResult Unlock(int id)
        {
            int actingUser = 1;
            _userService.UnlockUser(id, actingUser);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "User account unlocked"));
        }

        /// <summary>
        /// Fetches all data for the 3-step User Wizard (Identity, Companies, Permissions).
        /// </summary>
        [HttpGet("wizard-data")]
        public IActionResult GetWizardData([FromQuery] int userId = 0, [FromQuery] string roleIds = "")
        {
            var data = _userService.GetUserWizardData(userId, roleIds);
            return Ok(ApiResponse<UserWizardViewModel>.SuccessResponse(data));
        }

        /// <summary>
        /// Final 3-step wizard submission.
        /// Performs transactional save of user, companies, and permission overrides.
        /// </summary>
        [HttpPost("save-wizard")]
        public IActionResult SaveWizard([FromBody] UserUpsertRequest request)
        {
            // TODO: Get acting user from JWT
            int actingUser = 1;
            var response = _userService.SaveUserWizard(request, actingUser);

            if (response.Result > 0)
                return Ok(ApiResponse<bool>.SuccessResponse(true, response.Message));
            
            return BadRequest(ApiResponse<bool>.ErrorResponse(response.Message));
        }
    }
}
