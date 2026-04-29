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
    public class SubjectApiController : ControllerBase
    {
        private readonly ISubjectService _subjectService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;

        public SubjectApiController(ISubjectService subjectService, ICompanyService companyService, ISessionService sessionService)
        {
            _subjectService = subjectService;
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
                return Ok(ApiResponse<List<MstSubjectViewModel>>.SuccessResponse(new List<MstSubjectViewModel>()));

            var data = _subjectService.GetAllSubjects(companyId, sessionId, includeDeleted);
            return Ok(ApiResponse<List<MstSubjectViewModel>>.SuccessResponse(data));
        }

        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            var data = _subjectService.GetSubjectByID(id);
            if (data == null) return NotFound(ApiResponse<MstSubjectViewModel>.ErrorResponse("Subject not found"));
            return Ok(ApiResponse<MstSubjectViewModel>.SuccessResponse(data));
        }

        [HttpPost("Upsert")]
        public IActionResult Upsert([FromBody] MstSubjectUpsertRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            int userId = GetCurrentUserId();
            int companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
            int sessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;

            if (companyId == 0 || sessionId == 0)
                return BadRequest(ApiResponse<dynamic>.ErrorResponse("Current company or session not set."));

            var (success, message) = _subjectService.UpsertSubject(request, companyId, sessionId, userId);
            return Ok(new { success, message });
        }

        [HttpPost("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            int userId = GetCurrentUserId();
            var (success, message) = _subjectService.DeleteSubject(id, userId);
            return Ok(new { success, message });
        }

        [HttpPost("ToggleStatus")]
        public IActionResult ToggleStatus(int id, bool isActive)
        {
            int userId = GetCurrentUserId();
            var (success, message) = _subjectService.ToggleSubjectStatus(id, isActive, userId);
            return Ok(new { success, message });
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }
    }
}
