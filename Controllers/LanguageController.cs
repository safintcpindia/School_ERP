using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;
using SchoolERP.Net.Services.Clients;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers
{
    /// <summary>
    /// This class handles HTTP routing and API requests for LanguageController.
    /// </summary>
    public class LanguageController : Controller
    {
        private readonly ILanguageClientService _languageClient;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/Language";

        public LanguageController(ILanguageClientService languageClient, IUserMenuPermissionService menuPerm)
        {
            _languageClient = languageClient;
            _menuPerm = menuPerm;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _languageClient.GetAllAsync();
            var model = new MstLanguagePageViewModel
            {
                Languages = response.Success ? response.Data : new List<MstLanguageViewModel>()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetLanguage(int id)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit languages." });

            var response = await _languageClient.GetByIDAsync(id);
            if (!response.Success) return Json(new { success = false, message = response.Message });
            return Json(new { success = true, data = response.Data });
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] MstLanguageUpsertRequest request)
        {
            var isCreate = request.LanguageId <= 0;
            if (isCreate && !_menuPerm.Has(User, MenuPath, "Add"))
                return Json(new { success = false, message = "You do not have permission to add languages." });
            if (!isCreate && !_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit languages." });

            var response = await _languageClient.UpsertAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to change language status." });

            var response = await _languageClient.ToggleStatusAsync(id, isActive);
            return Json(new { success = response.Success, message = response.Message });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!_menuPerm.Has(User, MenuPath, "Delete"))
                return Json(new { success = false, message = "You do not have permission to delete languages." });

            var response = await _languageClient.DeleteAsync(id);
            return Json(new { success = response.Success, message = response.Message });
        }
    }
}
