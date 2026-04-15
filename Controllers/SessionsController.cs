using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services.Clients;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SchoolERP.Net.Controllers
{
    /// <summary>
    /// This class handles HTTP routing and API requests for SessionsController.
    /// </summary>
    public class SessionsController : Controller
    {
        private readonly ISessionClientService _sessionClient;

        public SessionsController(ISessionClientService sessionClient)
        {
            _sessionClient = sessionClient;
        }

        /// <summary>
        /// Resolves the list of existing academic or financial operating cycles.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var response = await _sessionClient.GetAllAsync();
            var model = new MstSessionPageViewModel
            {
                Sessions = response.Success ? response.Data : new List<MstSessionViewModel>()
            };
            return View(model);
        }

        /// <summary>
        /// Extracts singular session timelines (Start and End mapping) for modifications.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSession(int id)
        {
            var response = await _sessionClient.GetByIDAsync(id);
            if (!response.Success) return Json(new { success = false, message = response.Message });
            return Json(new { success = true, data = response.Data });
        }

        /// <summary>
        /// Records new constraints and calendar bounds for a system operational session.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] MstSessionUpsertRequest request)
        {
            var response = await _sessionClient.UpsertAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Freezes or reactivates historic log blocks for a given temporal session.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            var response = await _sessionClient.ToggleStatusAsync(id, isActive);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Irreversibly drops a drafted temporal phase. Blocked if actively attached to transactions.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _sessionClient.DeleteAsync(id);
            return Json(new { success = response.Success, message = response.Message });
        }
    }
}
