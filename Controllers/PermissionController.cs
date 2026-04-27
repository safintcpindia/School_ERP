using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;

namespace SchoolERP.Net.Controllers
{
    /// <summary>
    /// This controller manages the different types of actions users are allowed to perform (like 'Add', 'Edit', or 'Delete') across the system.
    /// </summary>
    public class PermissionController : Controller
    {
        private readonly IUserManagementService _userMgmtService;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/Permission";

        public PermissionController(IUserManagementService userMgmtService, IUserMenuPermissionService menuPerm)
        {
            _userMgmtService = userMgmtService;
            _menuPerm = menuPerm;
        }

        /// <summary>
        /// Shows the main page with a list of all defined permissions.
        /// </summary>
        public IActionResult Index()
        {
            var model = new MstUserManagementPageViewModel
            {
                Permissions = _userMgmtService.GetAllPermissions()
            };
            return View(model);
        }

        /// <summary>
        /// Gets the details of a specific permission type so you can view or edit it.
        /// </summary>
        [HttpGet]
        public IActionResult GetPermission(int permissionId)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit permissions." });

            var permission = _userMgmtService.GetPermissionByID(permissionId);
            if (permission == null) return Json(new { success = false, message = "Permission not found" });
            return Json(new { success = true, permission = permission });
        }

        /// <summary>
        /// Saves a new permission type or updates an existing one with the details you provided.
        /// </summary>
        [HttpPost]
        public IActionResult Save([FromBody] MstPermissionUpsertRequest request)
        {
            var isCreate = request.PermissionID <= 0;
            if (isCreate && !_menuPerm.Has(User, MenuPath, "Add"))
                return Json(new { success = false, message = "You do not have permission to add permissions." });
            if (!isCreate && !_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit permissions." });

            var userId = _menuPerm.GetCurrentUserId(User);
            if (userId <= 0)
                return Json(new { success = false, message = "You must be signed in to save." });

            var result = _userMgmtService.UpsertPermission(request, userId, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0");
            return Json(new { success = result.success, message = result.message });
        }

        /// <summary>
        /// Turns a specific permission type on or off, determining if it can be assigned to roles.
        /// </summary>
        [HttpPost]
        public IActionResult ToggleStatus(int permissionId, bool isActive)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to change permission status." });

            var userId = _menuPerm.GetCurrentUserId(User);
            if (userId <= 0)
                return Json(new { success = false, message = "You must be signed in." });

            var result = _userMgmtService.TogglePermissionStatus(permissionId, isActive, userId, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0");
            return Json(new { success = result.success, message = result.message });
        }

        /// <summary>
        /// Permanently removes a permission type from the system's records.
        /// </summary>
        [HttpPost]
        public IActionResult Delete(int permissionId)
        {
            if (!_menuPerm.Has(User, MenuPath, "Delete"))
                return Json(new { success = false, message = "You do not have permission to delete permissions." });

            var userId = _menuPerm.GetCurrentUserId(User);
            if (userId <= 0)
                return Json(new { success = false, message = "You must be signed in." });

            var result = _userMgmtService.DeletePermission(permissionId, userId, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0");
            return Json(new { success = result.success, message = result.message });
        }
    }
}
