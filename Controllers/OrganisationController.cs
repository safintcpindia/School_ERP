using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;
using SchoolERP.Net.Services.Clients;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers
{
    /// <summary>
    /// This class handles HTTP routing and API requests for OrganisationController.
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

        public async Task<IActionResult> Index()
        {
            var response = await _organisationClient.GetAllOrganisationsAsync();
            var model = new OrganisationPageViewModel
            {
                Organisations = response.Success ? response.Data : new System.Collections.Generic.List<OrganisationViewModel>()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrganisation(int id)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit organisations." });

            var response = await _organisationClient.GetOrganisationByIDAsync(id);
            if (!response.Success) return Json(new { success = false, message = response.Message });
            return Json(new { success = true, data = response.Data });
        }

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

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to change organisation status." });

            var response = await _organisationClient.ToggleStatusAsync(id, isActive);
            return Json(new { success = response.Success, message = response.Message });
        }

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
