using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
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

        public MasterMenuController(IMenuClientService menuClient)
        {
            _menuClient = menuClient;
        }

        /// <summary>
        /// Retrieves the organizational UI layouts (the navigation lists).
        /// Extracts Parent-Child hierarchical trees.
        /// </summary>
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

        /// <summary>
        /// Fetches the payload defining an individual menu/folder map interaction.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMenu(int menuId)
        {
            var response = await _menuClient.GetMenuByIdAsync(menuId);
            if (!response.Success) return Json(new { success = false, message = response.Message });
            return Json(new { success = true, menu = response.Data });
        }

        /// <summary>
        /// Synchronizes changes or enacts newly registered links under the master UI framework sidebar.
        /// Implements distinct ModelState enforcement compared to custom validation architectures elsewhere.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] MenuUpsertRequest request)
        {
            // Verify annotations attached directly inside the MenuUpsertRequest object.
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(new { success = false, message = errors });
            }

            var response = await _menuClient.SaveMenuAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Disables a UI master menu option, dynamically restricting all sub-linked role access capabilities.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int menuId, bool isActive)
        {
            var response = await _menuClient.ToggleStatusAsync(menuId, isActive);
            return Json(new { success = response.Success, message = response.Message });
        }
    }
}
