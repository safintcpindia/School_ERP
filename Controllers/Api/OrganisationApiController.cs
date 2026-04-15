using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;
using SchoolERP.Net.Services;
using System.Security.Claims;

namespace SchoolERP.Net.Controllers.Api
{
   // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    /// <summary>
    /// Processes physical campuses, chains, or district descriptors binding the domain mapping.
    /// </summary>
    public class OrganisationApiController : ControllerBase
    {
        private readonly IOrganisationService _organisationService;

        public OrganisationApiController(IOrganisationService organisationService)
        {
            _organisationService = organisationService;
        }

        /// <summary>
        /// Reads all organizational blocks (allows opting into reading deactivated elements).
        /// </summary>
        [HttpGet("GetAll")]
        public IActionResult GetAll(bool includeDeleted = false)
        {
            var data = _organisationService.GetAllOrganisations(includeDeleted);
            return Ok(ApiResponse<List<OrganisationViewModel>>.SuccessResponse(data));
        }

        /// <summary>
        /// Queries detailed mapping constants for a single specific branch code.
        /// </summary>
        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            var data = _organisationService.GetOrganisationByID(id);
            if (data == null) return NotFound(ApiResponse<OrganisationViewModel>.ErrorResponse("Organisation slice not found"));
            return Ok(ApiResponse<OrganisationViewModel>.SuccessResponse(data));
        }

        /// <summary>
        /// Forces insertion or metadata changes to a campus properties structure.
        /// </summary>
        [HttpPost("Upsert")]
        public IActionResult Upsert([FromBody] OrganisationUpsertRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            int userId = GetCurrentUserId();
            var (success, message) = _organisationService.UpsertOrganisation(request, userId);
            return Ok(new { success, message });
        }

        /// <summary>
        /// Structurally removes an organization block hierarchy.
        /// </summary>
        [HttpPost("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            int userId = GetCurrentUserId();
            var (success, message) = _organisationService.DeleteOrganisation(id, userId);
            return Ok(new { success, message });
        }

        /// <summary>
        /// Suppresses visibility for a division natively without triggering destructive cleanses.
        /// </summary>
        [HttpPost("ToggleStatus")]
        public IActionResult ToggleStatus(int id, bool isActive)
        {
            int userId = GetCurrentUserId();
            var (success, message) = _organisationService.ToggleOrganisationStatus(id, isActive, userId);
            return Ok(new { success, message });
        }

        /// <summary>
        /// Identifies the actor context from session streams.
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1; // Fallback to 1 for demo/superadmin
        }
    }
}
