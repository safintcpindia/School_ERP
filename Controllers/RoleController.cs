using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services.Clients;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers
{
    /// <summary>
    /// Controller responsible for managing Application Roles, their statuses, and associated permission matrices.
    /// It interfaces with the backend IRoleClientService via API calls.
    /// </summary>
    public class RoleController : Controller
    {
        private readonly IRoleClientService _roleClient;

        public RoleController(IRoleClientService roleClient)
        {
            _roleClient = roleClient;
        }

        /// <summary>
        /// Serves the default Role Management view and injects the initial load of roles.
        /// </summary>
        /// <returns>The Razor View bound to MstUserManagementPageViewModel</returns>
        public async Task<IActionResult> Index()
        {
            // Fetch all roles via the client service wrapper
            var response = await _roleClient.GetAllRolesAsync();
            var model = new MstUserManagementPageViewModel
            {
                // Fallback to empty list if the API request fails to avoid NullReference exceptions in the view
                Roles = response.Success ? response.Data : new System.Collections.Generic.List<MstRoleViewModel>()
            };
            return View(model);
        }

        /// <summary>
        /// Retrieves a single role's detail model via AJAX GET for population in UI modals or edit forms.
        /// </summary>
        /// <param name="roleId">The targeted Role ID</param>
        [HttpGet]
        public async Task<IActionResult> GetRole(int roleId)
        {
            var response = await _roleClient.GetRoleByIdAsync(roleId);
            
            // Return failure state appropriately mapped for frontend JSON parsers
            if (!response.Success) return Json(new { success = false, message = response.Message });
            
            return Json(new { success = true, role = response.Data });
        }

        /// <summary>
        /// Receives form POST data to create or update a Role's structural definition.
        /// Returns the newly affected roleId for cascading operations (like assigning permissions right away).
        /// </summary>
        /// <param name="request">The Role's data payload</param>
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] MstRoleUpsertRequest request)
        {
            var response = await _roleClient.SaveRoleAsync(request);
            
            // Note: response.Data contains the modified or inserted roleId as configured by the backend service.
            return Json(new { success = response.Success, message = response.Message, roleId = response.Data });
        }

        /// <summary>
        /// Toggles a role's Active status flag, effectively disabling or enabling all users mapped to this role.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int roleId, bool isActive)
        {
            var response = await _roleClient.ToggleStatusAsync(roleId, isActive);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Gets the structured tree of manageable actions/permissions for a given Role ID.
        /// Typically rendered in a hierarchical matrix table with Read/Write toggles.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPermissions(int roleId)
        {
            var response = await _roleClient.GetPermissionsAsync(roleId);
            if (!response.Success) return Json(new { success = false, message = response.Message });
            
            // Pre-process null data fallback
            return Json(new { success = true, data = response.Data ?? new System.Collections.Generic.List<RoleMenuPermissionViewModel>() });
        }

        /// <summary>
        /// Submits the configured checkbox states from the Role Permissions tree matrix back to the database.
        /// Purges old mappings and inserts the active mapped permissions via a structured datatype mechanism in SQL.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SavePermissions([FromBody] MstRolePermissionSaveRequest request)
        {
            var response = await _roleClient.SavePermissionsAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }
    }
}
