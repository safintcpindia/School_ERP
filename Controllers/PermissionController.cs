using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;

namespace SchoolERP.Net.Controllers
{
    /// <summary>
    /// This class handles HTTP routing and API requests for PermissionController.
    /// </summary>
    public class PermissionController : Controller
    {
        private readonly IUserManagementService _userMgmtService;

        public PermissionController(IUserManagementService userMgmtService)
        {
            _userMgmtService = userMgmtService;
        }

        /// <summary>
        /// Instantiates the direct view layer mapped against the legacy User Management Service.
        /// Exposes all distinct permission identifiers currently recorded in the registry master.
        /// </summary>
        public IActionResult Index()
        {
            // The view loads statically from the direct database service layer 
            // instead of using a standalone client API proxy object.
            var model = new MstUserManagementPageViewModel
            {
                Permissions = _userMgmtService.GetAllPermissions()
            };
            return View(model);
        }

        /// <summary>
        /// Extracts an individual permission record for mutation rendering block scenarios.
        /// </summary>
        [HttpGet]
        public IActionResult GetPermission(int permissionId)
        {
            var permission = _userMgmtService.GetPermissionByID(permissionId);
            if (permission == null) return Json(new { success = false, message = "Permission not found" });
            return Json(new { success = true, permission = permission });
        }

        /// <summary>
        /// Overrides or seeds a new logical permission into the module.
        /// Grabs the connection IP context for audit logging.
        /// </summary>
        [HttpPost]
        public IActionResult Save([FromBody] MstPermissionUpsertRequest request)
        {
            // Note: Hardcoded '1' indicates a bypass or system admin user context fallback override.
            var result = _userMgmtService.UpsertPermission(request, 1, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0");
            return Json(new { success = result.success, message = result.message });
        }

        /// <summary>
        /// Controls whether a permission behaves or drops from UI checks without destructive database deletes.
        /// </summary>
        [HttpPost]
        public IActionResult ToggleStatus(int permissionId, bool isActive)
        {
            var result = _userMgmtService.TogglePermissionStatus(permissionId, isActive, 1, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0");
            return Json(new { success = result.success, message = result.message });
        }

        /// <summary>
        /// Destructively deletes a root permission boundary mapping. Cascades apply.
        /// </summary>
        [HttpPost]
        public IActionResult Delete(int permissionId)
        {
            var result = _userMgmtService.DeletePermission(permissionId, 1, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0");
            return Json(new { success = result.success, message = result.message });
        }
    }
}
