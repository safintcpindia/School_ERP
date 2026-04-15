using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
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

        public UserTypeController(IUserTypeClientService userTypeClient)
        {
            _userTypeClient = userTypeClient;
        }

        /// <summary>
        /// Hydrates the User Type management grid index view.
        /// Extracts all types from the backend service to pre-populate the HTML table.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var response = await _userTypeClient.GetAllAsync();
            var model = new MstUserManagementPageViewModel
            {
                UserTypes = response.Success ? response.Data : new System.Collections.Generic.List<MstUserTypeViewModel>()
            };
            return View(model);
        }

        /// <summary>
        /// Gets a specific User Type via AJAX for populating edit side-panels or modals.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUserType(int typeId)
        {
            var response = await _userTypeClient.GetByIdAsync(typeId);
            if (!response.Success) return Json(new { success = false, message = response.Message });
            return Json(new { success = true, userType = response.Data });
        }

        /// <summary>
        /// Submits the user type configurations to the database.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] MstUserTypeUpsertRequest request)
        {
            var response = await _userTypeClient.SaveAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Modifies the active toggle flag for a specific User Type.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int typeId, bool isActive)
        {
            var response = await _userTypeClient.ToggleStatusAsync(typeId, isActive);
            return Json(new { success = response.Success, message = response.Message });
        }
    }
}
