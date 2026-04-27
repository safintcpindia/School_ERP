using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;
using SchoolERP.Net.Services.Clients;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers
{
    /// <summary>
    /// This controller manages the organization settings, allowing you to set up and maintain the details of different school campuses or branches.
    /// </summary>
    public class OrganisationController : Controller
    {
        private readonly IOrganisationClientService _organisationClient;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/Organisation";

        public OrganisationController(IOrganisationClientService organisationClient, IUserMenuPermissionService menuPerm)
        {
            _organisationClient = organisationClient;
            _menuPerm = menuPerm;
        }

        /// <summary>
        /// Shows the main list of all organizations or campuses registered in the system.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var response = await _organisationClient.GetAllOrganisationsAsync();
            var model = new OrganisationPageViewModel
            {
                Organisations = response.Success ? response.Data : new System.Collections.Generic.List<OrganisationViewModel>()
            };
            return View(model);
        }

        /// <summary>
        /// Gets the details of a specific organization so you can view or edit its information.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetOrganisation(int id)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit organisations." });

            var response = await _organisationClient.GetOrganisationByIDAsync(id);
            if (!response.Success) return Json(new { success = false, message = response.Message });
            return Json(new { success = true, data = response.Data });
        }

        /// <summary>
        /// Saves a new organization's details or updates an existing one with the information you provided.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] OrganisationUpsertRequest request)
        {
            var isCreate = request.OrganisationID <= 0;
            if (isCreate && !_menuPerm.Has(User, MenuPath, "Add"))
                return Json(new { success = false, message = "You do not have permission to add organisations." });
            if (!isCreate && !_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit organisations." });

            var response = await _organisationClient.UpsertOrganisationAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Turns an organization's active status on or off, determining if it's currently in use.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to change organisation status." });

            var response = await _organisationClient.ToggleStatusAsync(id, isActive);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Permanently removes an organization record from the system.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!_menuPerm.Has(User, MenuPath, "Delete"))
                return Json(new { success = false, message = "You do not have permission to delete organisations." });

            var response = await _organisationClient.DeleteOrganisationAsync(id);
            return Json(new { success = response.Success, message = response.Message });
        }
    }
}
