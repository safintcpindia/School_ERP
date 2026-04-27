using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;
using SchoolERP.Net.Services.Clients;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers
{
    /// <summary>
    /// This controller manages the academic sessions (like '2023-24' or '2024-25'), allowing you to define the school years used in the system.
    /// </summary>
    public class SessionsController : Controller
    {
        private readonly ISessionClientService _sessionClient;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/Sessions";

        public SessionsController(ISessionClientService sessionClient, IUserMenuPermissionService menuPerm)
        {
            _sessionClient = sessionClient;
            _menuPerm = menuPerm;
        }

        /// <summary>
        /// Shows the main list of all academic sessions defined in the system.
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
        /// Gets the details of a specific academic session so you can view or edit its dates and name.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSession(int id)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit sessions." });

            var response = await _sessionClient.GetByIDAsync(id);
            if (!response.Success) return Json(new { success = false, message = response.Message });
            return Json(new { success = true, data = response.Data });
        }

        /// <summary>
        /// Saves a new academic session or updates an existing one with the dates and name you provided.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] MstSessionUpsertRequest request)
        {
            var isCreate = request.SessionId <= 0;
            if (isCreate && !_menuPerm.Has(User, MenuPath, "Add"))
                return Json(new { success = false, message = "You do not have permission to add sessions." });
            if (!isCreate && !_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit sessions." });

            var response = await _sessionClient.UpsertAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Turns an academic session on or off, determining if it can be selected for work.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to change session status." });

            var response = await _sessionClient.ToggleStatusAsync(id, isActive);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Permanently removes an academic session from the system's records.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!_menuPerm.Has(User, MenuPath, "Delete"))
                return Json(new { success = false, message = "You do not have permission to delete sessions." });

            var response = await _sessionClient.DeleteAsync(id);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Sets the chosen academic session as the 'active' one for the current user's session.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SetCurrent([FromBody] SetCurrentSessionRequest request)
        {
            if (request == null || request.SessionId <= 0)
                return Json(new { success = false, message = "Invalid session selection." });

            var response = await _sessionClient.SetCurrentSessionAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }
    }
}
