using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;
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
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/Role";

        public RoleController(IRoleClientService roleClient, IUserMenuPermissionService menuPerm)
        {
            _roleClient = roleClient;
            _menuPerm = menuPerm;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _roleClient.GetAllRolesAsync();
            var model = new MstUserManagementPageViewModel
            {
                Roles = response.Success ? response.Data : new System.Collections.Generic.List<MstRoleViewModel>()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetRole(int roleId)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit roles." });

            var response = await _roleClient.GetRoleByIdAsync(roleId);

            if (!response.Success) return Json(new { success = false, message = response.Message });

            return Json(new { success = true, role = response.Data });
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] MstRoleUpsertRequest request)
        {
            var isCreate = request.RoleID <= 0;
            if (isCreate && !_menuPerm.Has(User, MenuPath, "Add"))
                return Json(new { success = false, message = "You do not have permission to add roles." });
            if (!isCreate && !_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit roles." });

            var response = await _roleClient.SaveRoleAsync(request);

            return Json(new { success = response.Success, message = response.Message, roleId = response.Data });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int roleId, bool isActive)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to change role status." });

            var response = await _roleClient.ToggleStatusAsync(roleId, isActive);
            return Json(new { success = response.Success, message = response.Message });
        }

        [HttpGet]
        public async Task<IActionResult> GetPermissions(int roleId)
        {
            if (!_menuPerm.Has(User, MenuPath, "View"))
                return Json(new { success = false, message = "You do not have permission to view role permissions." });

            var response = await _roleClient.GetPermissionsAsync(roleId);
            if (!response.Success) return Json(new { success = false, message = response.Message });

            return Json(new { success = true, data = response.Data ?? new System.Collections.Generic.List<RoleMenuPermissionViewModel>() });
        }

        [HttpPost]
        public async Task<IActionResult> SavePermissions([FromBody] MstRolePermissionSaveRequest request)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to update role permissions." });

            var response = await _roleClient.SavePermissionsAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int roleId)
        {
            if (!_menuPerm.Has(User, MenuPath, "Delete"))
                return Json(new { success = false, message = "You do not have permission to delete roles." });

            var response = await _roleClient.DeleteRoleAsync(roleId);
            return Json(new { success = response.Success, message = response.Message });
        }
    }
}
