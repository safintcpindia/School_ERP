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
    public class SubjectGroupApiController : ControllerBase
    {
        private readonly ISubjectGroupService _service;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/Academics/SubjectGroup";

        public SubjectGroupApiController(ISubjectGroupService service, ICompanyService companyService, ISessionService sessionService, IUserMenuPermissionService menuPerm)
        {
            _service = service;
            _companyService = companyService;
            _sessionService = sessionService;
            _menuPerm = menuPerm;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll(bool includeDeleted = false)
        {
            int userId = GetCurrentUserId();
            int companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
            int sessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;

            if (companyId == 0 || sessionId == 0)
                return Ok(ApiResponse<List<MstSubjectGroupViewModel>>.SuccessResponse(new List<MstSubjectGroupViewModel>()));

            var data = _service.GetAll(companyId, sessionId, includeDeleted);
            return Ok(ApiResponse<List<MstSubjectGroupViewModel>>.SuccessResponse(data));
        }

        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            var data = _service.GetByID(id);
            if (data == null) return NotFound(ApiResponse<MstSubjectGroupViewModel>.ErrorResponse("Subject Group not found"));
            return Ok(ApiResponse<MstSubjectGroupViewModel>.SuccessResponse(data));
        }

        [HttpPost("Upsert")]
        public IActionResult Upsert([FromBody] MstSubjectGroupUpsertRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            int userId = GetCurrentUserId();

            var isCreate = request.SubjectGroupID <= 0;
            if (isCreate && !_menuPerm.Has(User, MenuPath, "Add"))
                return Ok(new { success = false, message = "You do not have permission to add subject groups." });
            if (!isCreate && !_menuPerm.Has(User, MenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to edit subject groups." });

            int companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
            int sessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;

            if (companyId == 0 || sessionId == 0)
                return BadRequest(ApiResponse<dynamic>.ErrorResponse("Current company or session not set."));

            var (success, message) = _service.Upsert(request, companyId, sessionId, userId);
            return Ok(new { success, message });
        }

        [HttpPost("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            if (!_menuPerm.Has(User, MenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete subject groups." });

            int userId = GetCurrentUserId();
            var (success, message) = _service.Delete(id, userId);
            return Ok(new { success, message });
        }

        [HttpPost("ToggleStatus")]
        public IActionResult ToggleStatus(int id, bool isActive)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to change status." });

            int userId = GetCurrentUserId();
            var (success, message) = _service.ToggleStatus(id, isActive, userId);
            return Ok(new { success, message });
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }
    }
}
