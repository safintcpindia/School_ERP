using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;
using SchoolERP.Net.Services.Clients;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers
{
    /// <summary>
    /// This class handles HTTP routing and API requests for SessionsController.
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

        public async Task<IActionResult> Index()
        {
            var response = await _sessionClient.GetAllAsync();
            var model = new MstSessionPageViewModel
            {
                Sessions = response.Success ? response.Data : new List<MstSessionViewModel>()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetSession(int id)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit sessions." });

            var response = await _sessionClient.GetByIDAsync(id);
            if (!response.Success) return Json(new { success = false, message = response.Message });
            return Json(new { success = true, data = response.Data });
        }

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

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to change session status." });

            var response = await _sessionClient.ToggleStatusAsync(id, isActive);
            return Json(new { success = response.Success, message = response.Message });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!_menuPerm.Has(User, MenuPath, "Delete"))
                return Json(new { success = false, message = "You do not have permission to delete sessions." });

            var response = await _sessionClient.DeleteAsync(id);
            return Json(new { success = response.Success, message = response.Message });
        }

        [HttpPost]
        public IActionResult SetCurrent([FromBody] SetCurrentSessionRequest request)
        {
            if (request == null || request.SessionId <= 0)
                return Json(new { success = false, message = "Invalid session selection." });

            Response.Cookies.Append(
                "CurrentSessionId",
                request.SessionId.ToString(),
                new CookieOptions
                {
                    Path = "/",
                    HttpOnly = false,
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = Request.IsHttps
                });

            return Json(new { success = true });
        }
    }
}
