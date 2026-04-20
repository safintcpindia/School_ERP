using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;
using SchoolERP.Net.Services.Clients;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers
{
    /// <summary>
    /// This class handles HTTP routing and API requests for MasterMenuController.
    /// </summary>
    public class MasterMenuController : Controller
    {
        private readonly IMenuClientService _menuClient;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/MasterMenu";

        public MasterMenuController(IMenuClientService menuClient, IUserMenuPermissionService menuPerm)
        {
            _menuClient = menuClient;
            _menuPerm = menuPerm;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _menuClient.GetAllMenusAsync();
            var menus = response.Success ? response.Data : new System.Collections.Generic.List<MenuViewModel>();

            var model = new MenusPageViewModel
            {
                Menus = menus,
                ParentMenus = menus.Where(m => m.ParentID == null).ToList()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetMenu(int menuId)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit menus." });

            var response = await _menuClient.GetMenuByIdAsync(menuId);
            if (!response.Success) return Json(new { success = false, message = response.Message });
            return Json(new { success = true, menu = response.Data });
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] MenuUpsertRequest request)
        {
            var isCreate = request.MenuID <= 0;
            if (isCreate && !_menuPerm.Has(User, MenuPath, "Add"))
                return Json(new { success = false, message = "You do not have permission to add menus." });
            if (!isCreate && !_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit menus." });

            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(new { success = false, message = errors });
            }

            var response = await _menuClient.SaveMenuAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int menuId, bool isActive)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to change menu status." });

            var response = await _menuClient.ToggleStatusAsync(menuId, isActive);
            return Json(new { success = response.Success, message = response.Message });
        }
    }
}
