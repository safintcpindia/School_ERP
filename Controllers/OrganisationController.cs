using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
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

        public OrganisationController(IOrganisationClientService organisationClient)
        {
            _organisationClient = organisationClient;
        }

        /// <summary>
        /// Initiates the primary Organizational dashboard view.
        /// Contains branches/system identifiers globally structuring the ERP root.
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
        /// Retreives base attributes of a given organizational branch.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetOrganisation(int id)
        {
            var response = await _organisationClient.GetOrganisationByIDAsync(id);
            if (!response.Success) return Json(new { success = false, message = response.Message });
            return Json(new { success = true, data = response.Data });
        }

        /// <summary>
        /// Pushes structural configuration (Name, Address, Registration IDs) to the master service.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] OrganisationUpsertRequest request)
        {
            var response = await _organisationClient.UpsertOrganisationAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Soft switches branch availability without removing the historic audit trail.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            var response = await _organisationClient.ToggleStatusAsync(id, isActive);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Exterminates an organization segment (Warning: Hazardous operation typically reserved for fresh resets).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _organisationClient.DeleteOrganisationAsync(id);
            return Json(new { success = response.Success, message = response.Message });
        }
    }
}
