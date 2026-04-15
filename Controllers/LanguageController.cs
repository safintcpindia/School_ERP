using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
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

        public LanguageController(ILanguageClientService languageClient)
        {
            _languageClient = languageClient;
        }

        /// <summary>
        /// Hydrates the general Language catalog (Supported Languages Index).
        /// Data powers the translation drop-downs elsewhere.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var response = await _languageClient.GetAllAsync();
            var model = new MstLanguagePageViewModel
            {
                Languages = response.Success ? response.Data : new List<MstLanguageViewModel>()
            };
            return View(model);
        }

        /// <summary>
        /// Pulls language metadata (Name, Native Code) by its PK sequence.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetLanguage(int id)
        {
            var response = await _languageClient.GetByIDAsync(id);
            if (!response.Success) return Json(new { success = false, message = response.Message });
            return Json(new { success = true, data = response.Data });
        }

        /// <summary>
        /// Registers a new linguistic framework or patches an active descriptor.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] MstLanguageUpsertRequest request)
        {
            var response = await _languageClient.UpsertAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Deactivates a language, stripping it from selectable UI components transparently.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            var response = await _languageClient.ToggleStatusAsync(id, isActive);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Attempts to erase the local language definitions. (Might block due to foreign keys).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _languageClient.DeleteAsync(id);
            return Json(new { success = response.Success, message = response.Message });
        }
    }
}
