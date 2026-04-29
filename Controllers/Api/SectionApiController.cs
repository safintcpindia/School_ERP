using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;
using SchoolERP.Net.Services;
using System.Collections.Generic;
using System.Security.Claims;

namespace SchoolERP.Net.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SectionApiController : ControllerBase
    {
        private readonly ISectionService _sectionService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;

        public SectionApiController(ISectionService sectionService, ICompanyService companyService, ISessionService sessionService)
        {
            _sectionService = sectionService;
            _companyService = companyService;
            _sessionService = sessionService;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll(bool includeDeleted = false)
        {
            int userId = GetCurrentUserId();
            int companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
            int sessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;

            if (companyId == 0 || sessionId == 0)
                return Ok(ApiResponse<List<MstSectionViewModel>>.SuccessResponse(new List<MstSectionViewModel>()));

            var data = _sectionService.GetAllSections(companyId, sessionId, includeDeleted);
            return Ok(ApiResponse<List<MstSectionViewModel>>.SuccessResponse(data));
        }

        [HttpGet("GetByClass/{classId}")]
        public IActionResult GetByClass(int classId)
        {
            var data = _sectionService.GetSectionsByClass(classId);
            return Ok(ApiResponse<List<MstSectionViewModel>>.SuccessResponse(data));
        }

        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            var data = _sectionService.GetSectionByID(id);
            if (data == null) return NotFound(ApiResponse<MstSectionViewModel>.ErrorResponse("Section not found"));
            return Ok(ApiResponse<MstSectionViewModel>.SuccessResponse(data));
        }

        [HttpPost("Upsert")]
        public IActionResult Upsert([FromBody] MstSectionUpsertRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            int userId = GetCurrentUserId();
            int companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
            int sessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;

            if (companyId == 0 || sessionId == 0)
                return BadRequest(ApiResponse<dynamic>.ErrorResponse("Current company or session not set."));

            var (success, message) = _sectionService.UpsertSection(request, companyId, sessionId, userId);
            return Ok(new { success, message });
        }

        [HttpPost("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            int userId = GetCurrentUserId();
            var (success, message) = _sectionService.DeleteSection(id, userId);
            return Ok(new { success, message });
        }

        [HttpPost("ToggleStatus")]
        public IActionResult ToggleStatus(int id, bool isActive)
        {
            int userId = GetCurrentUserId();
            var (success, message) = _sectionService.ToggleSectionStatus(id, isActive, userId);
            return Ok(new { success, message });
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }
    }
}
