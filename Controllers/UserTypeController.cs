using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;
using SchoolERP.Net.Services.Clients;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers
{
    /// <summary>
    /// This class handles HTTP routing and API requests for UserTypeController.
    /// </summary>
    public class UserTypeController : Controller
    {
        private readonly IUserTypeClientService _userTypeClient;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/UserType";

        public UserTypeController(IUserTypeClientService userTypeClient, IUserMenuPermissionService menuPerm)
        {
            _userTypeClient = userTypeClient;
            _menuPerm = menuPerm;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _userTypeClient.GetAllAsync();
            var model = new MstUserManagementPageViewModel
            {
                UserTypes = response.Success ? response.Data : new System.Collections.Generic.List<MstUserTypeViewModel>()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserType(int typeId)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit user types." });

            var response = await _userTypeClient.GetByIdAsync(typeId);
            if (!response.Success) return Json(new { success = false, message = response.Message });
            return Json(new { success = true, userType = response.Data });
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] MstUserTypeUpsertRequest request)
        {
            var isCreate = request.UserTypeID <= 0;
            if (isCreate && !_menuPerm.Has(User, MenuPath, "Add"))
                return Json(new { success = false, message = "You do not have permission to add user types." });
            if (!isCreate && !_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit user types." });

            var response = await _userTypeClient.SaveAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int typeId, bool isActive)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to change user type status." });

            var response = await _userTypeClient.ToggleStatusAsync(typeId, isActive);
            return Json(new { success = response.Success, message = response.Message });
        }
    }
}
